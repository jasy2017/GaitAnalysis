from fastapi import FastAPI, UploadFile, File
from fastapi.responses import JSONResponse
from pydantic import BaseModel  # permite definir los modelos de datos
import cv2
from pathlib import Path  # permite trabajar con rutas de archivos
import os
from datetime import datetime
import numpy as np
import json
from database import db, connection
import tempfile
# from videoProcessing import coordinates_gaitAnalysis, angles_Gait
from gait_analysis import process_video
app = FastAPI()
video_path = "./"
video_counter = 0


class Patient(BaseModel):
    # id: int
    name: str
    lastname: str
    age: int
    weight: float
    height: float


class Test(BaseModel):
    # IdTest: int
    IdPatients: int
    VideoPath: str
    Date: str


class TestResult(BaseModel):
    # id: int
    idTest: int
    caderaX: str
    caderaY: str
    rodillaX: str
    rodillaY: str
    tobilloX: str
    tobilloY: str
    # antepieX: str
    # antepieY: str


class TestAngles(BaseModel):
    # id: int
    idTestAngle: int
    anguloCadera: str
    anguloRodilla: str
    anguloTobillo: str


#################################### Enpoints para la tabla test_results (coodernadas) ########################################
@app.get("/testresult/{idTest}", tags=["Resultado de Pruebas. Coordenadas"])
async def get_testresult(idTest: int):
    db.execute("SELECT * FROM testresults where idTest=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    # test_results = []

    test_results = {
        "Idtestresult": rows[0],
        "idTest": rows[1],
        "caderaX": rows[2],
        "caderaY": rows[3],
        "rodillaX": rows[4],
        "rodillaY": rows[5],
        "tobilloX": rows[6],
        "tobilloY": rows[7]
    }
    return test_results
# #################################################################################################################


@app.get("/testdisplacement/{idTest}", tags=["Desplazamiento"])
async def get_displacement(idTest: int):
    db.execute(
        "SELECT * FROM testdisplacement where idTestDisplacement=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    test_displacement = {
        "idMainTestDisplacementModel": rows[0],
        "idTestDisplacement": rows[1],
        "caderaDisplacement": rows[2],
        "rodillaDisplacement": rows[3],
        "tobilloDisplacement": rows[4],
    }
    return test_displacement
# #################################################################################################################


@app.get("/testangle/{idTest}", tags=["Ángulos de Pruebas"])
async def get_testangle(idTest: int):
    db.execute("SELECT * FROM testangle where idTestAngle=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    test_angles = {
        "IdMainTestAngle": rows[0],
        "idTestAngle": rows[1],
        "anguloCadera": rows[2],
        "anguloRodilla": rows[3],
        "anguloTobillo": rows[4],
    }
    return test_angles
# #################################################################################################################


@app.get("/testlinearvelocity/{idTest}", tags=["Velocidad lineal de Pruebas"])
async def get_testlinearvelocity(idTest: int):
    db.execute(
        "SELECT * FROM testlinearvelocity where idTestLinearVelocity=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    test_L_V = {
        "IdMainTestLinearVelocityModel": rows[0],
        "idTestLinearVelocity": rows[1],
        "caderaLinearVelocity": rows[2],
        "rodillaLinearVelocity": rows[3],
        "tobilloLinearVelocity": rows[4],
    }
    return test_L_V
#################################################################################################################


@app.get("/testlinearacceleration/{idTest}", tags=["Aceleración lineal de Pruebas"])
async def get_testlinearacceleration(idTest: int):
    db.execute(
        "SELECT * FROM testlinearacceleration where idTestLinearAcceleration=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    test_L_A = {
        "IdMainTestLinearAccelerationModel": rows[0],
        "idTestLinearAcceleration": rows[1],
        "caderaLinearAcceleration": rows[2],
        "rodillaLinearAcceleration": rows[3],
        "tobilloLinearAcceleration": rows[4],
    }
    return test_L_A
#################################################################################################################


@app.get("/testangularvelocity/{idTest}", tags=["Velocidad angular de Pruebas"])
async def get_testangularvelocity(idTest: int):
    db.execute(
        "SELECT * FROM testangularvelocity where idTestAngularVelocity=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    test_Ang_L = {
        "IdMainTestAngularVelocityModel": rows[0],
        "idTestAngularVelocity": rows[1],
        "caderaAngularVelocity": rows[2],
        "rodillaAngularVelocity": rows[3],
        "tobilloAngularVelocity": rows[4],
    }
    return test_Ang_L
#################################################################################################################


@app.get("/testangularacceleration/{idTest}", tags=["Aceleración angular de Pruebas"])
async def get_testangularacceleration(idTest: int):
    db.execute(
        "SELECT * FROM testangularacceleration where idTestAngularAcceleration=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    test_Ang_A = {
        "IdMainTestAngularAccelerationModel": rows[0],
        "idTestAngularAcceleration": rows[1],
        "caderaAngularAcceleration": rows[2],
        "rodillaAngularAcceleration": rows[3],
        "tobilloAngularAcceleration": rows[4],
    }
    return test_Ang_A
#################################################################################################################


@app.get("/testmoments/{idTest}", tags=["Momentos de Pruebas"])
async def get_testmoments(idTest: int):
    db.execute("SELECT * FROM testmoments where idTestMoment=%s", (idTest,))
    rows = db.fetchone()
    # print(rows)
    test_moments = {
        "IdMainTestMoment": rows[0],
        "idTestMoment": rows[1],
        "caderaMomentMrc": rows[3],
        "rodillaMomentMtr": rows[5],
        "tobilloMomentMmt": rows[7],
    }

    return test_moments


# #################################################################################################################

@app.post("/testresult/{idtest}/{weight}", tags=["Resultado de Pruebas"])
async def upload_coordinates(idtest: int, weight: float, test: Test):
    # print(f"peso: {weight}")
    # print(test.VideoPath)
    print("Analizando video")
    try:
        results = process_video(video_path+test.VideoPath, weight)

        coordinates = results["coordenadas"]
        displacement = results["desplazamientos"]
        angles = results["angulos"]
        Linearvelocities = results["velocidadesLineales"]
        Linearacceleration = results["aceleracionesLineales"]
        Angularvelocity = results["velocidadesAngulares"]
        Angularacceleration = results["aceleracionesAngulares"]
        moments = results["momentos"]

        cadera_X = json.dumps(coordinates[0])
        cadera_Y = json.dumps(coordinates[1])
        rodilla_X = json.dumps(coordinates[2])
        rodilla_Y = json.dumps(coordinates[3])
        tobillo_X = json.dumps(coordinates[4])
        tobillo_Y = json.dumps(coordinates[5])

        displacement_hip = json.dumps(displacement[0])
        displacement_knee = json.dumps(displacement[1])
        displacement_ankle = json.dumps(displacement[2])

        angles_hip = json.dumps(angles[0])
        angles_knee = json.dumps(angles[1])
        angle_ankle = json.dumps(angles[2])

        velocity_linear_hip = json.dumps(Linearvelocities[0])
        velocity_linear_knee = json.dumps(Linearvelocities[1])
        velocity_linear_ankle = json.dumps(Linearvelocities[2])

        acceleration_linear_hip = json.dumps(Linearacceleration[0])
        acceleration_linear_knee = json.dumps(Linearacceleration[1])
        acceleration_linear_ankle = json.dumps(Linearacceleration[2])

        velocity_angular_hip = json.dumps(Angularvelocity[0])
        velocity_angular_knee = json.dumps(Angularvelocity[1])
        velocity_angular_ankle = json.dumps(Angularvelocity[2])

        acceleration_angular_hip = json.dumps(Angularacceleration[0])
        acceleration_angular_knee = json.dumps(Angularacceleration[1])
        acceleration_angular_ankle = json.dumps(Angularacceleration[2])

        moment_hip_Mrc = json.dumps(moments[1])
        moment_knee_Mtr = json.dumps(moments[3])
        moment_ankle_Mmt = json.dumps(moments[5])

        data = (idtest, cadera_X, cadera_Y, rodilla_X,
                rodilla_Y, tobillo_X, tobillo_Y)
        sqlWrite = "INSERT INTO testresults(idTest,caderaX, caderaY, rodillaX, rodillaY, tobilloX, tobilloY) VALUES (%s,%s,%s,%s,%s,%s,%s)"
        db.execute(sqlWrite, data)

        data1 = (idtest, displacement_hip,
                 displacement_knee, displacement_ankle)
        sqlWrite1 = "INSERT INTO testdisplacement(idTestDisplacement,caderaDisplacement, rodillaDisplacement, tobilloDisplacement) VALUES (%s,%s,%s,%s)"
        db.execute(sqlWrite1, data1)

        data2 = (idtest, angles_hip, angles_knee, angle_ankle)
        sqlWrite2 = "INSERT INTO testangle(idTestAngle,anguloCadera, anguloRodilla, anguloTobillo) VALUES (%s,%s,%s,%s)"
        db.execute(sqlWrite2, data2)

        data3 = (idtest, velocity_linear_hip,
                 velocity_linear_knee, velocity_linear_ankle)
        sqlWrite3 = "INSERT INTO testlinearvelocity(idTestLinearVelocity,caderaLinearVelocity, rodillaLinearVelocity, tobilloLinearVelocity) VALUES (%s,%s,%s,%s)"
        db.execute(sqlWrite3, data3)

        data4 = (idtest, acceleration_linear_hip,
                 acceleration_linear_knee, acceleration_linear_ankle)
        sqlWrite4 = "INSERT INTO testlinearacceleration(idTestLinearAcceleration,caderaLinearAcceleration, rodillaLinearAcceleration, tobilloLinearAcceleration) VALUES (%s,%s,%s,%s)"
        db.execute(sqlWrite4, data4)

        data5 = (idtest, velocity_angular_hip,
                 velocity_angular_knee, velocity_angular_ankle)
        sqlWrite5 = "INSERT INTO testangularvelocity(idTestAngularVelocity,caderaAngularVelocity, rodillaAngularVelocity, tobilloAngularVelocity) VALUES (%s,%s,%s,%s)"
        db.execute(sqlWrite5, data5)

        data6 = (idtest, acceleration_angular_hip,
                 acceleration_angular_knee, acceleration_angular_ankle)
        sqlWrite6 = "INSERT INTO testangularacceleration(idTestAngularAcceleration,caderaAngularAcceleration, rodillaAngularAcceleration, tobilloAngularAcceleration) VALUES (%s,%s,%s,%s)"
        db.execute(sqlWrite6, data6)

        data7 = (idtest, moment_hip_Mrc, moment_knee_Mtr, moment_ankle_Mmt)
        sqlWrite7 = "INSERT INTO testmoments(idTestMoment,caderaMomentMrc, rodillaMomentMtr, tobilloMomentMmt) VALUES (%s,%s,%s,%s)"
        db.execute(sqlWrite7, data7)
        connection.commit()
        print("Fin del análisis del video")
    except Exception as e:
        print(f"Error: {e}")


########################################### Enpoints para la tabla test ########################################
# sube video y guarda la ruta en la base de datos


@app.post("/uploadvideo/{patientid}", tags=["Pruebas"])
async def create_upload_video(patientid: int, file: UploadFile = File(...)):
    print("Subiendo video")
    # Extrae el nombre base del archivo sin la extensión (video)
    base_name = os.path.splitext(file.filename)[0]
    # Extrae la extensión del archivo (.mp4)
    extension = os.path.splitext(file.filename)[1]
    counter = 1
    # Encuentra el siguiente número de archivo disponible
    while os.path.exists(f"{base_name}_{counter}{extension}"):
        counter += 1
    # Añade el contador al nombre del archivo
    file_path = Path(f"{base_name}_{counter}{extension}")
    with open(file_path, 'wb+') as buffer:
        buffer.write(await file.read())
    current_date = datetime.now()  # Fecha y hora actual
    test_table = (patientid, str(file_path),
                  current_date.strftime('%Y-%m-%d %H:%M:%S'))
    sqlWrite = "INSERT INTO test(idPatients,videoPath,date) VALUES (%s,%s,%s)"
    db.execute(sqlWrite, test_table)
    connection.commit()
    print("Video subido")


@app.get("/test", tags=["Pruebas"])
async def get_test():
    db.execute("SELECT * FROM test")
    rows = db.fetchall()
    test = []
    for row in rows:
        test_item = {
            "IdTest": row[0],
            "IdPatients": row[1],
            "VideoPath": row[2],
            "Date": row[3]
        }
        test.append(test_item)
    # print(test)
    return test


@app.get("/test/{patientid}", tags=["Pruebas"])
async def get_last_IDtest(patientid: int):
    query = "SELECT MAX(id) AS max_id FROM test WHERE idPatients = %s;"
    db.execute(query, (patientid,))
    result = db.fetchone()
    return result[0]


@app.delete("/test/{testid}", tags=["Pruebas"])
async def delete_test(testid: int):
    query = "DELETE FROM test WHERE id = %s"
    db.execute(query, (testid,))
    connection.commit()


@app.get("/test/list/{patientid}", tags=["Pruebas"])
async def get_last_IDtest(patientid: int):
    query = "SELECT * FROM test WHERE idPatients = %s;"
    db.execute(query, (patientid,))
    result = db.fetchall()
    tests = []
    for row in result:
        test_item = {
            "IdTest": row[0],
            "IdPatients": row[1],
            "VideoPath": row[2],
            "Date": row[3]
        }
        tests.append(test_item)
    return tests


###################### Enpoints para la tabla pacientes #############################
@app.post("/patients", tags=["Pacientes"])
async def add_patient(patient: Patient):
    sqlWrite = "INSERT INTO patients(name,lastname,age,weight,height) VALUES (%s,%s,%s,%s,%s)"
    patients = (patient.name, patient.lastname,
                patient.age, patient.weight, patient.height)
    db.execute(sqlWrite, patients)
    connection.commit()


@app.get("/patients", tags=["Pacientes"])
async def get_patients():
    db.execute("SELECT * FROM patients")
    rows = db.fetchall()
    patients = []
    for row in rows:
        patient = {
            "id": row[0],
            "Name": row[1],
            "Lastname": row[2],
            "Age": row[3],
            "Weight": row[4],
            "Height": row[5]
        }
        patients.append(patient)
    return patients


@app.get("/patients/{patientid}", tags=["Pacientes"])
async def get_patient(patientid: int):
    query = "SELECT * FROM patients WHERE id = %s"
    db.execute(query, (patientid,))
    tuple = db.fetchone()
    patient = {
        "Id": tuple[0],
        "Name": tuple[1],
        "Lastname": tuple[2],
        "Age": tuple[3],
        "Weight": tuple[4],
        "Height": tuple[5]
    }

    return patient


@app.delete("/patients/{patientid}", tags=["Pacientes"])
async def delete_patient(patientid: int):
    query = "DELETE FROM patients WHERE id = %s"
    db.execute(query, (patientid,))
    connection.commit()


@app.put("/patients/{patientid}", tags=["Pacientes"])
async def update_patient(patientid: int, patient: Patient):
    query = "UPDATE patients SET name = %s, lastname = %s, age = %s, weight = %s, height = %s WHERE id = %s"
    data = (patient.name, patient.lastname, patient.age,
            patient.weight, patient.height, patientid)
    db.execute(query, data)
    connection.commit()
