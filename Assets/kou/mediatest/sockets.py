import socket
import cv2
import struct
import time

def get_ip_address():
    import netifaces
    interfaces = netifaces.interfaces()
    main_interfaces = ['wlan0', 'Wi-Fi', 'en0']
    for interface in main_interfaces:
        if interface in interfaces:
            addresses = netifaces.ifaddresses(interface)
            if netifaces.AF_INET in addresses:
                ipv4_info = addresses[netifaces.AF_INET][0]
                return ipv4_info['addr']
    gws = netifaces.gateways()
    default_interface = gws['default'][netifaces.AF_INET][1]
    addresses = netifaces.ifaddresses(default_interface)
    if netifaces.AF_INET in addresses:
        ipv4_info = addresses[netifaces.AF_INET][0]
        return ipv4_info['addr']
    return None

def send_image(conn):
    cap = cv2.VideoCapture(0)
    try:
        while True:
            ret, frame = cap.read()
            if not ret:
                print("Failed to capture image")
                continue
            frame = cv2.resize(frame, (640, 480))  # Unity 側の Texture2D サイズに合わせる
            ret, buffer = cv2.imencode('.jpg', frame)
            if not ret:
                print("Failed to encode image")
                continue
            data = buffer.tobytes()
            size = len(data)
            print(f"Sending image of size: {size}")

            try:
                # 最初にサイズを送信する
                conn.sendall(struct.pack(">I", size))  # ">I"は符号なし整数（4バイト、ビッグエンディアン）
                conn.sendall(data)
            except Exception as e:
                print(f"Error during send: {e}")
                break

            time.sleep(0.1)  # 送信間隔を少し空けることで、Unity クライアントが受信しやすくする

    except KeyboardInterrupt:
        print("KeyboardInterrupt detected. Exiting...")
    except Exception as e:
        print(f"Error: {e}")
    finally:
        cap.release()
        conn.close()

HOST = get_ip_address()  # 現在接続されているWi-FiのIPアドレスを取得
PORT = 65432

if HOST is None:
    print("IPアドレスを取得できませんでした")
else:
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((HOST, PORT))
        s.listen()
        print(f"Waiting for a connection on {HOST}:{PORT}...")
        conn, addr = s.accept()
        with conn:
            print('Connected by', addr)
            send_image(conn)
