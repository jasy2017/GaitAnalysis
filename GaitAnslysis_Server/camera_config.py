# Código para encontrar la relación entre metro y pixel de la
# cámara móvil de Samsung Galaxy A70, considerando 3.5 metros
# de distancia de la cámara al objeto

import numpy as np

# resolución horizontal y vertical considerando una toma horizontal


def px_to_meter(distancia_objeto_m=1.5, resolucion_horizontal=3840, resolucion_vertical=2160):
    # DATOS CONOCIDOS
    diagonal_sensor_inch = 1 / 2.8  # Formato del sensor en pulgadas
    diagonal_sensor_mm = diagonal_sensor_inch * 25.4  # Conversión a mm
    aspect_ratio_width = 4  # Relación de aspecto ancho
    aspect_ratio_height = 3  # Relación de aspecto alto
    distancia_focal_mm = 3.92  # Distancia focal Samsung Galaxy A70

    # Calcular el ángulo de la diagonal
    aspect_ratio_angle = np.arctan(aspect_ratio_height / aspect_ratio_width)

    # Calcular dimensiones del sensor
    sensor_height_mm = diagonal_sensor_mm * np.sin(aspect_ratio_angle)
    sensor_width_mm = diagonal_sensor_mm * np.cos(aspect_ratio_angle)

    # CÁLCULAR CAMPO VISUAL
    # Calcular FOV horizontal y vertical
    fov_horizontal = 2 * np.arctan(sensor_width_mm / (2 * distancia_focal_mm))
    fov_vertical = 2 * np.arctan(sensor_height_mm / (2 * distancia_focal_mm))

    # Calcular el ancho y alto del área capturada en metros
    ancho_area_m = 2 * distancia_objeto_m * np.tan(fov_horizontal / 2)
    alto_area_m = 2 * distancia_objeto_m * np.tan(fov_vertical / 2)

    # RELACIÓN PIXEL-METRO
    h_meter = alto_area_m/resolucion_vertical
    w_meter = ancho_area_m/resolucion_horizontal
    return h_meter, w_meter
