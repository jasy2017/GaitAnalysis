import mysql.connector
connection = mysql.connector.connect(
    user='root', password='password', host='127.0.0.1', database='database', port='3306')
db = connection.cursor()

connection.commit()
