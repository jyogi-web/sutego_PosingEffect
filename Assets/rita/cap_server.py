import socket
import cv2
import numpy as np
import struct
import time

#IPアドレス取得
hostname = socket.gethostname()
local_ip = socket.gethostbyname(hostname)
print("local_ipを取得："+str(local_ip))

# サーバーのホストとポートを設定
HOST = local_ip
PORT = 50007

# カメラのキャプチャ
cap = cv2.VideoCapture(0)

# TCP/IPソケットを作成し、サーバーに接続
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
s.listen(1)
conn, addr = s.accept()

print('Connected by', addr)

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

        # ESCが押されたら終了
        if cv2.waitKey(1) & 0xFF == 27:
            break
except Exception as e:
    print(f"送信中にエラーが発生しました: {e}")

# ソケットを閉じる
conn.close()
s.close()
