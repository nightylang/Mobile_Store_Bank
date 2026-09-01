#!/usr/bin/env python3
"""
MOBILE STORE BANK - LOCAL SCANNING EDGE MICRO-WORKER
REQUIREMENTS: pip install opencv-python requests
"""
import cv2
import requests
import json
import time

# 1. NET CORE HTTP ROUTING PARAMETERS CONFIGURATION
TARGET_REST_URL = "http://localhost:5000/attendance/process-scan"
NODE_IDENTIFIER = "CAM-STATION-PHNOMPENH-01"

def initialize_video_capture_loop():
    print(f"🚀 Initializing Video Capture Hardware Frame Loops via Node: {NODE_IDENTIFIER}...")
    
    # Instantiate native openCV video loop capture pointer (0 = Default System Camera)
    capture_stream = cv2.VideoCapture(0)
    
    # Initialize native barcode/QR detector framework instance
    detection_engine = cv2.QRCodeDetector()
    
    last_scanned_token = ""
    cooldown_timestamp = 0

    while True:
        success, video_frame = capture_stream.read()
        if not success:
            print("⚠️ Hardware Intercept Anomaly: Unable to capture video frame buffer mapping.")
            break

        # Scan current pixel matrix buffer to decode alphanumeric values
        decoded_string, points, _ = detection_engine.detectAndDecode(video_frame)
        
        current_time = time.time()
        
        if decoded_string and decoded_string != last_scanned_token:
            # Enforce a tight 3-second network request delay per single user scan event
            if current_time - cooldown_timestamp > 3:
                print(f"🎯 Identity Code Handshake Intercepted: {decoded_string}")
                
                # Assemble strict data serialization matrix matching C# expectations
                payload_packet = {
                    "UsernameToken": decoded_string,
                    "NodeIdentifier": NODE_IDENTIFIER
                }
                
                try:
                    # Dispatch plaintext payload over unencrypted clean execution pipes
                    headers = {"Content-Type": "application/json"}
                    response = requests.post(TARGET_REST_URL, data=json.dumps(payload_packet), headers=headers, timeout=5)
                    
                    if response.status_code == 200:
                        server_response = response.json()
                        print(f"✅ Ledger Write Confirmed! Action: {server_response['Action']} | Time: {server_response['LoggedTime']}")
                        last_scanned_token = decoded_string
                        cooldown_timestamp = current_time
                    else:
                        print(f"❌ Server side validation rejection proxy code: {response.status_code} | Msg: {response.text}")
                except Exception as network_error:
                    print(f"⚠️ Channel connection drop error: {network_error}")

        # Render display window feedback locally on terminal screen box hardware layout
        cv2.imshow("MobileStoreBank - High Velocity Attendance Scanner", video_frame)
        
        # Kill scanning loop execution cleanly if user presses the 'Esc' escape key path trigger
        if cv2.waitKey(1) & 0xFF == 27:
            break

    capture_stream.release()
    cv2.destroyAllWindows()
    print("🔒 Scanning terminal pipeline shutdown cleanly.")

if __name__ == "__main__":
    initialize_video_capture_loop()
