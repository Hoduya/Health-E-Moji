import sys
import cv2
import mediapipe as mp
import math
import numpy as np
from math import dist, sqrt
import matplotlib.pyplot as plt

COUNTER = 0
TOTAL_BLINKS = 0

# Eye indices for calc ear
LEFT_EYE = [ 160, 144, 158, 153, 33, 133 ]
RIGHT_EYE = [ 387, 373, 385, 380, 263, 362 ]

# FACE indices for calc angle
FACE = [1, 9, 57, 130, 287, 359]

mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_holistic = mp.solutions.holistic


def getLandmarksCoordinate(image, results, idx):
    image_height, image_width= image.shape[:2]
    coordinates = (results[idx].x * image_width, results[idx].y * image_height)
    
    return coordinates

# Euclaidean distance to calculate the distance between the two points
def euclaideanDistance(point, point1):
    x, y = point
    x1, y1 = point1
    distance = sqrt((x1 - x)**2 + (y1 - y)**2)

    return distance

#calculate ear
def eye_aspect_ratio(faceLm, eye):
    # compute the euclidean distances between the two sets of
    # vertical eye landmarks (x, y)-coordinates
    p1 = [faceLm.landmark[eye[0]].x, faceLm.landmark[eye[0]].y]
    p2 = [faceLm.landmark[eye[1]].x, faceLm.landmark[eye[1]].y]
    p3 = [faceLm.landmark[eye[2]].x, faceLm.landmark[eye[2]].y]
    p4 = [faceLm.landmark[eye[3]].x, faceLm.landmark[eye[3]].y]
    p5 = [faceLm.landmark[eye[4]].x, faceLm.landmark[eye[4]].y]
    p6 = [faceLm.landmark[eye[5]].x, faceLm.landmark[eye[5]].y]

    # compute the eye aspect ratio
    A = euclaideanDistance(p1, p2)
    B = euclaideanDistance(p3, p4)
    C = euclaideanDistance(p5, p6)
    
    if C == 0:
        return 0

    ear = (A + B) / (2.0 * C)

    return ear


def rotation_matrix_to_angles(rotation_matrix):

    x = math.atan2(rotation_matrix[2, 1], rotation_matrix[2, 2])
    y = math.atan2(-rotation_matrix[2, 0], math.sqrt(rotation_matrix[0, 0] ** 2 +
                                                     rotation_matrix[1, 0] ** 2))
    z = math.atan2(rotation_matrix[1, 0], rotation_matrix[0, 0])
    return np.array([x, y, z]) * 180. / math.pi

# For webcam input:
cap = cv2.VideoCapture(0, cv2.CAP_DSHOW)

with mp_holistic.Holistic(
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5) as holistic:
    while cap.isOpened():
        success, image = cap.read()
        if not success:
            print("Ignoring empty camera frame.")
        # If loading a video, use 'break' instead of 'continue'.
            continue

        # To improve performance, optionally mark the image as not writeable to
        # pass by reference.
        image.flags.writeable = False
        image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        image = cv2.flip(image, 1)
        results = holistic.process(image)
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

        h, w, _ = image.shape
        face_coordinate_in_image = []
        eye_coordinate_in_image = []
        body_coordinate_in_image = []

        if results.face_landmarks:
            
            # Calc ear for eye blink detection
            left_ear = eye_aspect_ratio(results.face_landmarks, LEFT_EYE)
            right_ear = eye_aspect_ratio(results.face_landmarks, RIGHT_EYE)
            ear = (left_ear + right_ear) /2

            # Calc distance between chin and sholder line for forward head posture
            shoulderY = (results.pose_landmarks.landmark[11].y * h + results.pose_landmarks.landmark[12].y * h) / 2
            chinY = getLandmarksCoordinate(image, results.face_landmarks.landmark, 199)[1]
            csDist = shoulderY - chinY

            # Draw landmark annotation on the image.
            image.flags.writeable = True 
            mp_drawing.draw_landmarks(
                image,
                results.face_landmarks,
                mp_holistic.FACEMESH_CONTOURS,
                landmark_drawing_spec=None,
            )

            mp_drawing.draw_landmarks(
                image,
                results.pose_landmarks,
                mp_holistic.POSE_CONNECTIONS,
            )
                        
            points = []
            for i in (35, 168, 192, 282, 52, 416):
                points.append(getLandmarksCoordinate(image, results.face_landmarks.landmark, i))
            
            
            print(f'FromPython {ear:.3f} {csDist:.1f}')
            sys.stdout.flush()

        # Flip the image horizontally for a selfie-view display.
        cv2.namedWindow('Healthy-E-Moji Setting')
        cv2.moveWindow('Healthy-E-Moji Setting', 0, 0)
        cv2.imshow('Healthy-E-Moji Setting', image)
        
        if cv2.waitKey(5) & 0xFF == 27:
            break

cap.release()
