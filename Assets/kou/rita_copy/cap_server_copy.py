import socket
import cv2
import mediapipe as mp
import numpy as np
import struct
import json
import time

# IPアドレス取得
hostname = socket.gethostname()
local_ip = socket.gethostbyname(hostname)
print("local_ipを取得："+str(local_ip))

# サーバーのホストとポートを設定
HOST = local_ip
PORT = 50007
JSONPORT = 50008 

# カメラのキャプチャ
cap = cv2.VideoCapture(0)

# TCP/IPソケットを作成し、サーバーに接続
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
s.listen(1)
conn, addr = s.accept()

json_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
json_socket.bind((HOST, JSONPORT))
json_socket.listen(1)
json_conn, json_addr = json_socket.accept()

print('Connected by', addr)
print('JSON Connected by', json_addr)

# Mediapipeのポーズ検出器の初期化
mp_pose = mp.solutions.pose
pose = mp_pose.Pose()

def detect_pose(image):
    image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    results = pose.process(image_rgb)
    
    landmarks = []
    if results.pose_landmarks:
        for landmark in results.pose_landmarks.landmark:
            landmarks.append({
                "x": landmark.x,
                "y": landmark.y,
                "z": landmark.z,
                "visibility": landmark.visibility
            })
    return landmarks

try:
    while cap.isOpened():
        success, frame = cap.read()
        if not success:
            break

        # 画像をバイト配列に変換
        _, buffer = cv2.imencode('.jpg', frame)
        byte_data = buffer.tobytes()

        # 画像データの長さを送信（4バイトの長さ情報）
        conn.sendall(struct.pack('>I', len(byte_data)))  # ビッグエンディアンでエンコード

        # 画像データを送信
        conn.sendall(byte_data)

        # ポーズを検出
        landmarks = detect_pose(frame)

        # ポーズデータをJSON形式に変換して送信
        landmarks_json = json.dumps(landmarks)
        json_conn.sendall(landmarks_json.encode('utf-8'))

        # ESCが押されたら終了
        if cv2.waitKey(1) & 0xFF == 27:
            break
except Exception as e:
    print(f"送信中にエラーが発生しました: {e}")

# ソケットを閉じる
conn.close()
s.close()
json_conn.close()
json_socket.close()
cap.release()
