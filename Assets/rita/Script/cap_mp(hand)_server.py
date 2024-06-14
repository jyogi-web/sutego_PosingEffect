import socket
import cv2
import numpy as np
import struct
import json
import mediapipe as mp

# IPアドレス取得
hostname = socket.gethostname()
local_ip = socket.gethostbyname(hostname)
print("local_ipを取得：" + str(local_ip))

# サーバーのホストとポートを設定
HOST = local_ip
PORT = 50007  # キャプチャ画像用
PORT2 = 50008  # 座標データ用

# MediaPipeのhandsモジュールを初期化
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=2,
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5)

# 描画用のユーティリティを初期化
mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles

# カメラのキャプチャ
cap = cv2.VideoCapture(0)

# TCP/IPソケットを作成し、サーバーに接続
print("画像データ用TCP/IPソケットを作成しています")
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind((HOST, PORT))
print("画像データ用ソケット作成完了")

print("座標データ用TCP/IPソケットを作成しています")
s2 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s2.bind((HOST, PORT2))
print("座標データ用ソケット作成完了")

print("クライアントの接続を待ち受け中...")
s.listen(1)
conn, addr = s.accept()
print("画像データ用クライアント接続成功")
print('Connected by', addr)

s2.listen(1)
conn2, addr2 = s2.accept()
print("座標データ用クライアント接続成功")
print('connected by', addr2)

try:
    print("キャプチャした画像を転送中...")
    while cap.isOpened():
        success, frame = cap.read()
        if not success:
            break

        # BGR画像をRGBに変換
        frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        # 結果の描画をスキップするために画像を書き込み不可に設定
        frame.flags.writeable = False
        # MediaPipe Handsの処理を実行
        results = hands.process(frame)
        # 画像を書き込み可能に戻す
        frame.flags.writeable = True
        # RGB画像をBGRに変換
        frame = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)

        # ハンドランドマークの描画
        if results.multi_hand_landmarks:
            hand_landmarks = {}
            for hand_index, hand_landmarks in enumerate(results.multi_hand_landmarks):
                for id, landmark in enumerate(hand_landmarks.landmark):
                    hand_landmarks[f'hand_{hand_index}_landmark_{id}'] = {
                        'x': landmark.x,
                        'y': landmark.y,
                        'z': landmark.z
                    }
                mp_drawing.draw_landmarks(
                    frame,
                    hand_landmarks,
                    mp_hands.HAND_CONNECTIONS,
                    landmark_drawing_spec=mp_drawing_styles.get_default_hand_landmarks_style())

        # 画像をバイト配列に変換
        _, buffer = cv2.imencode('.jpg', frame)
        byte_data = buffer.tobytes()
        # 画像データの長さを送信（4バイトの長さ情報）
        conn.sendall(struct.pack('>I', len(byte_data)))  # ビッグエンディアンでエンコード
        # 画像データを送信
        conn.sendall(byte_data)

        # JSONデータを文字列に変換してエンコード
        json_data = json.dumps(hand_landmarks).encode('utf-8')
        # JSONデータを転送
        conn2.sendall(json_data)

        # ESCが押されたら終了
        if cv2.waitKey(1) & 0xFF == 27:
            break
except Exception as e:
    print(f"送信中にエラーが発生しました: {e}")

# ソケットを閉じる
conn.close()
s.close()
