import mediapipe as mp
import json
import socket
import cv2

# ポーズ検出関数
def detect_pose_landmarks(image):
    mp_pose = mp.solutions.pose

    # ポーズ検出器を初期化
    with mp_pose.Pose(static_image_mode=False, model_complexity=1, enable_segmentation=False, min_detection_confidence=0.5) as pose:
        # 画像をRGBに変換
        image_rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        # 画像を処理してランドマークを検出
        results = pose.process(image_rgb)

        if results.pose_landmarks:
            # ランドマークを辞書に格納
            landmarks = {}
            for idx, landmark in enumerate(results.pose_landmarks.landmark):
                # 座標を小数点第3位まで丸める
                landmarks[idx] = {'x': round(landmark.x, 3), 'y': round(landmark.y, 3), 'z': round(landmark.z, 3)}
            return landmarks
    return None

# ランドマークをJSON形式で送信する関数
def send_landmarks_as_json(landmarks, conn):
    if landmarks:
        # ランドマークをJSON形式に変換
        json_data = json.dumps(landmarks)
        # UnityにJSONデータを送信
        conn.sendall(json_data.encode())
        print("JSONデータをUnityに送信しました。")
    else:
        print("ポーズのランドマークが検出されませんでした。")

# カメラから画像をキャプチャ
cap = cv2.VideoCapture(0)

# サーバーの設定
HOST = '0.0.0.0'  # すべてのネットワークインターフェースからの接続を受け付ける
PORT = 65433      # 使用するポート番号
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server_socket:
    server_socket.bind((HOST, PORT))
    server_socket.listen()
    print(f"{HOST}:{PORT} で接続を待っています")

    # クライアントからの接続を待つ
    conn, addr = server_socket.accept()
    print(f"{addr} から接続されました")

    with conn:
        while True:
            ret, frame = cap.read()
            if not ret:
                print("画像のキャプチャに失敗しました")
                break

            # ポーズのランドマークを検出
            landmarks = detect_pose_landmarks(frame)

            # ランドマークをJSON形式でUnityに送信
            send_landmarks_as_json(landmarks, conn)

            # ランドマーク付きの画像を表示
            cv2.imshow('Pose Landmarks Detection', frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

# 接続を閉じる
conn.close()
cap.release()
cv2.destroyAllWindows()
