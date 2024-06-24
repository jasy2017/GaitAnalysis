from scipy.signal import savgol_filter
import math
import numpy as np
import mediapipe as mp
import cv2
from camera_config import px_to_meter


def transformar_coordenadas(x, y, altura):
    x_prime = x
    y_prime = altura - 1 - y
    return x_prime, y_prime


def gait_analysis(video_path):
    mp_drawing = mp.solutions.drawing_utils
    mp_pose = mp.solutions.pose

    # Cargar el video
    cap = cv2.VideoCapture(video_path)
    ancho_original = int(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
    # print(ancho_original)
    alto_original = int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))
    # print(alto_original)
    relacion = ancho_original / alto_original
    nuevo_alto = 680
    nuevo_ancho = int(nuevo_alto * relacion)

    # Distancia de la cámara a la persona
    distancia = 3  # metros
    nueva_distancia = distancia*(nuevo_ancho/ancho_original)

    # conversión de pixel a metro
    h_meter, w_meter = px_to_meter(nueva_distancia, nuevo_ancho, nuevo_alto)

    # Lista de centroides para los puntos claves
    list_centroides = {24: [], 26: [], 28: [], 30: [], 32: []}
    original_centroides = {24: [], 26: [], 28: [], 30: [], 32: []}
    fps = cap.get(cv2.CAP_PROP_FPS)
    total_frames = int(cap.get(cv2.CAP_PROP_FRAME_COUNT))

    # Imagen para dibujar la trayectoria
    trajectory_image = None

    with mp_pose.Pose(model_complexity=2, min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:
        while cap.isOpened():
            ret, frame = cap.read()
            if not ret:
                break

            frame = cv2.resize(frame, (nuevo_ancho, nuevo_alto))
            if trajectory_image is None:
                trajectory_image = frame.copy()
            image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            image.flags.writeable = False

            # Hacer la detección de pose
            results = pose.process(image)
            image.flags.writeable = True
            image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

            if results.pose_landmarks:
                for id in list_centroides.keys():
                    lm = results.pose_landmarks.landmark[id]
                    x = int(lm.x * nuevo_ancho)
                    y = int(lm.y * nuevo_alto)

                    # Guardar las coordenadas originales
                    original_x, original_y = x, y

                    # Transformar las coordenadas
                    x, y = transformar_coordenadas(x, y, nuevo_alto)

                    # Verificar la coherencia espacial
                    if list_centroides[id]:
                        last_point = list_centroides[id][-1]
                        if list_centroides[24][0][0] > (nuevo_ancho // 2):
                            if x > last_point[0]:
                                x, y = last_point  # Usar la última posición válida
                        else:
                            if x < last_point[0]:
                                x, y = last_point
                    list_centroides[id].append((x, y))

                    original_centroides[id].append((original_x, original_y))
                    # Graficar con las coordenadas originales
                    cv2.putText(frame, '{},{}'.format(original_x, original_y), (original_x, original_y),
                                2, 0.3, (255, 0, 0), 1, cv2.LINE_AA)
                    if len(original_centroides[id]) > 1 and (id == 24 or id == 26 or id == 28):
                        cv2.line(
                            trajectory_image, original_centroides[id][-2], original_centroides[id][-1], (0, 255, 0), 3)

            else:
                # Si MediaPipe no reconoce la pose en un fotograma, reeemplaza el valor no hallado por el último valor
                for id in list_centroides.keys():
                    if list_centroides[id]:
                        last_point = list_centroides[id][-1]
                        list_centroides[id].append(last_point)
                        original_last_point = original_centroides[id][-1]
                        original_centroides[id].append(original_last_point)

            # Superponer la imagen de trayectoria con el frame actual
            combined_image = cv2.addWeighted(
                frame, 0.7, trajectory_image, 0.3, 0)

            # Mostrar el frame
            cv2.imshow('MediaPipe Pose', combined_image)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

    cap.release()
    cv2.destroyAllWindows()
    return list_centroides, fps, total_frames, h_meter, w_meter


def calcular_angulo_tobillo(talon, tobillo, pie):
    # Vector talón a tobillo
    v1 = np.array([tobillo[0] - talon[0], tobillo[1] - talon[1]])
    # Vector tobillo a pie
    v2 = np.array([pie[0] - tobillo[0], pie[1] - tobillo[1]])

    # Producto punto de v1 y v2
    dot_product = np.dot(v1, v2)

    # Magnitudes de v1 y v2
    mag_v1 = np.linalg.norm(v1)
    mag_v2 = np.linalg.norm(v2)

    # Ángulo entre los vectores en radianes
    cos_theta = dot_product / (mag_v1 * mag_v2)
    # Asegurarse de que el valor esté en el rango válido para arccos
    cos_theta = np.clip(cos_theta, -1.0, 1.0)
    theta = np.arccos(cos_theta)

    # Convertir a grados
    angulo = np.degrees(theta)
    return angulo


def encontrar_interseccion(A, B, C):
    A = np.array(A)
    B = np.array(B)
    C = np.array(C)

    # Calcular los puntos medios de AB y AC
    mid_AB = (A + B) / 2
    mid_AC = (A + C) / 2

    # Calcular los radios de las circunferencias
    radius_AB = np.linalg.norm(B - A) / 2
    radius_AC = np.linalg.norm(C - A) / 2

    # Calcular las intersecciones de las circunferencias
    d = np.linalg.norm(mid_AB - mid_AC)
    a = (radius_AB**2 - radius_AC**2 + d**2) / (2 * d)
    h = np.sqrt(radius_AB**2 - a**2)

    P2 = mid_AB + a * (mid_AC - mid_AB) / d

    intersect1 = P2 + h * \
        np.array([mid_AC[1] - mid_AB[1], mid_AB[0] - mid_AC[0]]) / d
    intersect2 = P2 - h * \
        np.array([mid_AC[1] - mid_AB[1], mid_AB[0] - mid_AC[0]]) / d

    Mx = 4*A[0]-2*intersect2[0]
    My = 4*A[1]-2*intersect2[1]
    M = np.array([Mx, My])

    # Devolver el punto de intersección que no es A
    if np.allclose(intersect1, A):
        return intersect2, M
    else:
        return intersect1, M


def detectar_cambio_abrupto(p1, p2, p3):
    return (abs(p2[0] - p1[0]) > 2 * abs(p3[0] - p1[0])) or \
           (abs(p2[1] - p1[1]) > 2 * abs(p3[1] - p1[1]))


def corregir_punto(p1, p3):
    return ((p1[0] + p3[0]) // 2, (p1[1] + p3[1]) // 2)


def calcular_distancia(punto1, punto2):
    return math.sqrt((punto2[0]-punto1[0])**2 + (punto2[1]-punto1[1])**2)


def calcular_pendiente(punto1, punto2):
    return np.degrees(np.pi / 2 - np.arctan2((punto2[1] - punto1[1]), (punto2[0] - punto1[0])))


def calcular_angulo(punto1, punto2, punto3):
    a = calcular_distancia(punto1, punto2)
    b = calcular_distancia(punto2, punto3)
    c = calcular_distancia(punto3, punto1)
    return math.degrees(math.acos((a**2 + b**2 - c**2)/(2*a*b)))


def calcular_velocidad_savgol(positions, delta_t, window_length=5, polyorder=2):
    x = np.array([p[0] for p in positions])
    y = np.array([p[1] for p in positions])
    v_x = savgol_filter(x, window_length, polyorder, deriv=1, delta=delta_t)
    v_y = savgol_filter(y, window_length, polyorder, deriv=1, delta=delta_t)
    velocities = np.sqrt(v_x**2 + v_y**2)
    return velocities


def calcular_angulo_prueba(punto1, punto2):
    return np.degrees(np.arctan2((punto2[1] - punto1[1]), (punto2[0] - punto1[0])))


def calcular_aceleracion_savgol(velocities, delta_t, window_length=5, polyorder=2):
    accelerations = savgol_filter(
        velocities, window_length, polyorder, deriv=1, delta=delta_t)
    return accelerations


def calcular_velocidad_angular_savgol(angulos, delta_t, window_length=5, polyorder=2):
    velocidades_angulares = savgol_filter(
        angulos, window_length, polyorder, deriv=1, delta=delta_t)
    return velocidades_angulares


def calcular_aceleracion_angular_savgol(velocidades_angulares, delta_t, window_length=5, polyorder=2):
    aceleraciones_angulares = savgol_filter(
        velocidades_angulares, window_length, polyorder, deriv=1, delta=delta_t)
    return aceleraciones_angulares


def calcular_momento(segmento, tipo_momento, d, theta_segmento, W_RC, W_M, W_TI, W_P):
    if segmento == "Cadera-Rodilla":
        if tipo_momento == "Mc":
            return ((W_RC + W_M)*d - W_M*(d/2)) * np.sin(np.radians(theta_segmento))
        elif tipo_momento == "Mrc":
            return (-(W_RC)*d-W_M*(d/2)) * np.sin(np.radians(theta_segmento))
    elif segmento == "Rodilla-Tobillo":
        if tipo_momento == "Mrt":
            return ((W_RC+W_M+W_TI)*d - W_TI*(d/2)) * np.sin(np.radians(theta_segmento))
        elif tipo_momento == "Mtr":
            return (-(W_RC+W_M)*d-W_TI*(d/2)) * np.sin(np.radians(theta_segmento))
    elif segmento == "Tobillo-QMetatarso":
        if tipo_momento == "Mtm":
            return ((W_RC+W_M+W_TI+W_P)*d-W_P*(d/2)) * np.sin(np.radians(theta_segmento))
        elif tipo_momento == "Mmt":
            return (-(W_RC+W_M+W_TI)*d-W_P*(d/2)) * np.sin(np.radians(theta_segmento))


def corregir_puntos(puntos):
    puntos_corregidos = [puntos[0]]
    for i in range(1, len(puntos) - 1):
        p1, p2, p3 = puntos[i - 1], puntos[i], puntos[i + 1]
        if detectar_cambio_abrupto(p1, p2, p3):
            p2 = corregir_punto(p1, p3)
        puntos_corregidos.append(p2)
    puntos_corregidos.append(puntos[-1])
    return puntos_corregidos


def suavizar_coordenadas(centroides):
    x = np.array([p[0] for p in centroides])
    y = np.array([p[1] for p in centroides])
    x_suavizado = savgol_filter(x, 7, 2)
    y_suavizado = savgol_filter(y, 7, 2)
    return list(zip(x_suavizado, y_suavizado))


def gait_analysis_coordinate(dict_coordenadas, X, Y):
    # Tratamiento de datos: Interpolación de puntos faltantes y suavizado
    coordenadas_suavizados = {id_punto: suavizar_coordenadas(corregir_puntos(coordenadas))
                              for id_punto, coordenadas in dict_coordenadas.items()}

    def extraer_coordenadas(id_punto):
        x_coords = []
        y_coords = []
        for x, y in coordenadas_suavizados[id_punto]:
            # Escalamiento de coordenadas: px to meter
            x_coords.append(x * X)
            y_coords.append(y * Y)
        return x_coords, y_coords

    # Extracción y escalado de coordenadas para cada punto clave
    coordenadas = {}
    coordenadas_graf = {
        'x_24': [],
        'y_24': [],
        'x_26': [],
        'y_26': [],
        'x_28': [],
        'y_28': [],
        'x_30': [],
        'y_30': [],
        'x_32': [],
        'y_32': []}
    for id_punto in [24, 26, 28, 30, 32]:
        x_coords, y_coords = extraer_coordenadas(id_punto)
        coordenadas[id_punto] = list(zip(x_coords, y_coords))
        coordenadas_graf[f"x_{id_punto}"] = x_coords
        coordenadas_graf[f"y_{id_punto}"] = y_coords

    return coordenadas_graf['x_24'], coordenadas_graf['y_24'], coordenadas_graf['x_26'], coordenadas_graf['y_26'], coordenadas_graf['x_28'], coordenadas_graf['y_28'], \
        coordenadas_graf['x_30'], coordenadas_graf['y_30'], coordenadas_graf['x_32'], coordenadas_graf['y_32'], \
        coordenadas


def gait_analysis_displacement(x, y):
    desplazamiento = []
    coordenadas = zip(x, y)
    for coordenada in coordenadas:
        modulo = math.sqrt(coordenada[0]**2 + coordenada[1]**2)
        desplazamiento.append(modulo)
    return desplazamiento


# Velocidad lineal


def gait_analysis_linear_velocity(list_centroides, fps):
    velocidades = {id: calcular_velocidad_savgol(
        list_centroides[id], 1/fps) for id in list_centroides.keys()}
    velocity_hip = velocidades[24]
    velocity_knee = velocidades[26]
    velocity_ankle = velocidades[28]

    return velocity_hip, velocity_knee, velocity_ankle, velocidades


# Aceleración lineal
def gait_analysis_linear_acceleration(list_centroides, fps, velocidades):
    aceleraciones = {id: calcular_aceleracion_savgol(
        velocidades[id], 1/fps) for id in list_centroides.keys()}
    acceleration_hip = aceleraciones[24]
    acceleration_knee = aceleraciones[26]
    acceleration_ankle = aceleraciones[28]

    return acceleration_hip, acceleration_knee, acceleration_ankle


# Ángulo


def gait_analysis_angles(list_centroides, y_meter):
    copia_cadera = [(x, y+(100*y_meter)) for x, y in list_centroides[24]]

    angulos_cadera = []
    theta1__hip = []

    angulos_rodilla = []

    angulos_tobillo = []
    theta_3_list = []
    theta_ankle = []
    # print(list_centroides)
    for i in range(len(list_centroides[24])):
        if i < len(list_centroides[24]) and i < len(list_centroides[26]) and i < len(list_centroides[28]) and i < len(list_centroides[32]):
            M, D = encontrar_interseccion(
                list_centroides[28][i], list_centroides[30][i], list_centroides[32][i])

            # Ángulo cadera
            angulo_cadera = calcular_angulo(
                copia_cadera[i], list_centroides[24][i], list_centroides[26][i])
            angulos_cadera.append(angulo_cadera)

            ang_hip = calcular_pendiente(
                list_centroides[26][i], list_centroides[24][i])
            theta1__hip.append(ang_hip)

            # Ángulo rodilla
            angulo_rodilla = calcular_angulo(
                list_centroides[24][i], list_centroides[26][i], list_centroides[28][i])
            angulos_rodilla.append(177 - angulo_rodilla)
            # Ángulo tobillo
            angulo_tobillo = calcular_angulo_tobillo(
                D, list_centroides[28][i], list_centroides[26][i])-90
            angulos_tobillo.append(angulo_tobillo)

    return theta1__hip, angulos_rodilla, angulos_tobillo


# Ángulo entre vector peso y posición
def gait_analysis_angles_weight(list_centroides):
    pendientes_cadera = []
    pendientes_tobillo = []
    pendientes_rodilla = []

    for i in range(len(list_centroides[24])):
        if i < len(list_centroides[24]) and i < len(list_centroides[26]) and i < len(list_centroides[28]) and i < len(list_centroides[32]):
            # Ángulo
            pendiente_cadera = calcular_pendiente(
                list_centroides[24][i], list_centroides[26][i])
            pendientes_cadera.append(pendiente_cadera)

            pendiente_rodilla = calcular_pendiente(
                list_centroides[26][i], list_centroides[28][i])
            pendientes_rodilla.append(pendiente_rodilla)

            pendiente_tobillo = calcular_pendiente(
                list_centroides[28][i], list_centroides[32][i])
            pendientes_tobillo.append(pendiente_tobillo)

    return pendientes_cadera, pendientes_rodilla, pendientes_tobillo  # grados


# Velocidad angular
def gait_analysis_angular_velocity(angulos_cadera, angulos_rodilla, angulos_tobillo, fps):

    velocidades_angulares_cadera = calcular_velocidad_angular_savgol(
        angulos_cadera, 1/fps)

    velocidades_angulares_rodilla = calcular_velocidad_angular_savgol(
        angulos_rodilla, 1/fps)

    velocidades_angulares_tobillo = calcular_velocidad_angular_savgol(
        angulos_tobillo, 1/fps)
    velocidades_angulares_cadera = np.deg2rad(velocidades_angulares_cadera)
    velocidades_angulares_rodilla = np.deg2rad(velocidades_angulares_rodilla)
    velocidades_angulares_tobillo = np.deg2rad(velocidades_angulares_tobillo)

    return velocidades_angulares_cadera, velocidades_angulares_rodilla, velocidades_angulares_tobillo


# Aceleración angular
def gait_analysis_angular_acceleration(velocidades_angulares_cadera, velocidades_angulares_rodilla, velocidades_angulares_tobillo, fps):
    aceleraciones_angulares_cadera = calcular_aceleracion_angular_savgol(
        velocidades_angulares_cadera, 1/fps)

    aceleraciones_angulares_rodilla = calcular_aceleracion_angular_savgol(
        velocidades_angulares_rodilla, 1/fps)

    aceleraciones_angulares_tobillo = calcular_aceleracion_angular_savgol(
        velocidades_angulares_tobillo, 1/fps)

    return aceleraciones_angulares_cadera, aceleraciones_angulares_rodilla, aceleraciones_angulares_tobillo


def gait_analysis_moment(list_centroides, pendientes_cadera, pendientes_rodilla, pendientes_tobillo, W_RC, W_M, W_TI, W_P):

    distancias = {
        'Cadera-Rodilla': calcular_distancia(list_centroides[24][0], list_centroides[26][0]),
        'Rodilla-Tobillo': calcular_distancia(list_centroides[26][0], list_centroides[28][0]),
        'Tobillo-QMetatarso': calcular_distancia(list_centroides[28][0], list_centroides[32][0])
    }

    def calcular_momentos(segmento, tipos, distancias, pendientes):
        return {tipo: [calcular_momento(segmento, tipo, distancias[segmento], theta, W_RC, W_M, W_TI, W_P) for theta in pendientes]
                for tipo in tipos}

    momentos_cadera = calcular_momentos(
        'Cadera-Rodilla', ['Mc', 'Mrc'], distancias, pendientes_cadera)
    momentos_rodilla = calcular_momentos(
        'Rodilla-Tobillo', ['Mrt', 'Mtr'], distancias, pendientes_rodilla)
    momentos_tobillo = calcular_momentos(
        'Tobillo-QMetatarso', ['Mtm', 'Mmt'], distancias, pendientes_tobillo)

    return (momentos_cadera['Mc'], momentos_cadera['Mrc'],
            momentos_rodilla['Mrt'], momentos_rodilla['Mtr'],
            momentos_tobillo['Mtm'], momentos_tobillo['Mmt'])


def calcular_potencia(momentos, velocidades_angulares):
    # Potencia = Momento * Velocidad Angular
    potencias = [m * w for m, w in zip(momentos, velocidades_angulares)]
    return potencias


def calcular_trabajo(momentos, angulos):
    # Trabajo = Momento * Delta_Ángulo
    angulos_rad = np.radians(angulos)
    delta_theta = np.diff(angulos_rad)
    momentos = momentos[:len(delta_theta)]
    trabajos = [m * dt for m, dt in zip(momentos, delta_theta)]
    return trabajos


def process_video(video_path: str, weight: float):
    # Pesos y longitudes de los segmentos (valores arbitrarios para ajustar posteriormente)
    W_RC = (1-0.161)*weight  # Peso de la región cadera (N) (1-0.161)*Wtotal
    W_M = 0.1*weight   # Peso del muslo (N) 0.1
    W_TI = 0.0465*weight   # Peso de la pierna (N) 0.0465
    W_P = 0.0145*weight    # Peso del pie (N) 0.0145
    # Inicializar la solución de pose
    dict_coordenadas, fps, frames, x_meter, y_meter = gait_analysis(video_path)

    # posición
    x_coords_cadera, y_coords_cadera, x_coords_rodilla, y_coords_rodilla, x_coords_tobillo, y_coords_tobillo, x_coords_talon, y_coords_talon, x_coords_meta, y_coords_meta, dict_coordenadas = gait_analysis_coordinate(
        dict_coordenadas, x_meter, y_meter)
    # desplazamiento

    desp_cadera = gait_analysis_displacement(x_coords_cadera, y_coords_cadera)
    desp_rodilla = gait_analysis_displacement(
        x_coords_rodilla, y_coords_rodilla)
    desp_tobillo = gait_analysis_displacement(
        x_coords_tobillo, y_coords_tobillo)
    # ángulo
    angulos_cadera, angulos_rodilla, angulos_tobillo = gait_analysis_angles(
        dict_coordenadas, y_meter)
    # velocidad lineal
    velocity_hip, velocity_knee, velocity_ankle, velocidades = gait_analysis_linear_velocity(
        dict_coordenadas, fps)
    # aceleración lineal
    acceleration_hip, acceleration_knee, acceleration_ankle = gait_analysis_linear_acceleration(
        dict_coordenadas, fps, velocidades)
    # velocidad angular
    velocidades_angulares_cadera, velocidades_angulares_rodilla, velocidades_angulares_tobillo = gait_analysis_angular_velocity(
        angulos_cadera, angulos_rodilla, angulos_tobillo, fps)
    # aceleracion angular
    aceleraciones_angulares_cadera, aceleraciones_angulares_rodilla, aceleraciones_angulares_tobillo = gait_analysis_angular_acceleration(
        velocidades_angulares_cadera, velocidades_angulares_rodilla, velocidades_angulares_tobillo, fps)
    # momentos
    pendientes_cadera, pendientes_rodilla, pendientes_tobillo = gait_analysis_angles_weight(
        dict_coordenadas)
    momentos_cadera_Mc, momentos_cadera_Mrc, momentos_rodilla_Mrt, momentos_rodilla_Mtr, momentos_tobillo_Mtm, momentos_tobillo_Mmt = gait_analysis_moment(
        dict_coordenadas, pendientes_cadera, pendientes_rodilla, pendientes_tobillo, W_RC, W_M, W_TI, W_P)

    potencia_cadera = calcular_potencia(
        momentos_cadera_Mrc, velocidades_angulares_cadera)
    potencia_rodilla = calcular_potencia(
        momentos_rodilla_Mtr, velocidades_angulares_rodilla)
    potencia_tobillo = calcular_potencia(
        momentos_tobillo_Mmt, velocidades_angulares_tobillo)

    trabajo_cadera = calcular_trabajo(momentos_cadera_Mrc, angulos_cadera)
    trabajo_rodilla = calcular_trabajo(momentos_rodilla_Mtr, angulos_rodilla)
    trabajo_tobillo = calcular_trabajo(momentos_tobillo_Mmt, angulos_tobillo)

    return {
        "coordenadas": (x_coords_cadera, y_coords_cadera, x_coords_rodilla, y_coords_rodilla, x_coords_tobillo, y_coords_tobillo, x_coords_talon, y_coords_talon, x_coords_meta, y_coords_meta),
        "desplazamientos": (desp_cadera, desp_rodilla, desp_tobillo),
        "angulos": (angulos_cadera, angulos_rodilla, angulos_tobillo),
        # LIST
        "velocidadesLineales": (velocity_hip.tolist(), velocity_knee.tolist(), velocity_ankle.tolist()),
        # LIST
        "aceleracionesLineales": (acceleration_hip.tolist(), acceleration_knee.tolist(), acceleration_ankle.tolist()),
        # LIST
        "velocidadesAngulares": (velocidades_angulares_cadera.tolist(), velocidades_angulares_rodilla.tolist(), velocidades_angulares_tobillo.tolist()),
        # LIST
        "aceleracionesAngulares": (aceleraciones_angulares_cadera.tolist(), aceleraciones_angulares_rodilla.tolist(), aceleraciones_angulares_tobillo.tolist()),
        "momentos": (momentos_cadera_Mc, momentos_cadera_Mrc, momentos_rodilla_Mrt, momentos_rodilla_Mtr, momentos_tobillo_Mtm, momentos_tobillo_Mmt)
    }
