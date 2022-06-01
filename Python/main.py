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
        return 1

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
    min_detection_confidence=0.7,
    min_tracking_confidence=0.7) as holistic:
    ear_list = []
    csDist_list = []
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
        #image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

        # For calc yaw, roll, pitch
        face_coordinate_in_real_world = np.array([
            [285, 528, 200],
            [285, 371, 152],
            [197, 574, 128],
            [173, 425, 108],
            [360, 574, 128],
            [391, 425, 108]
        ], dtype=np.float64)

        h, w, _ = image.shape
        face_coordinate_in_image = []
        eye_coordinate_in_image = []
        body_coordinate_in_image = []

        if results.face_landmarks:


            # # 이모지 얼굴 회전을 위한
            for idx, lm in enumerate( results.face_landmarks.landmark):
                # 인중, 미간, 바깥 눈 , 바깥 입술
                if idx in FACE:
                    x, y = int(lm.x * w), int(lm.y * h)
                    face_coordinate_in_image.append([x, y])

            face_coordinate_in_image = np.array(face_coordinate_in_image,
                                                  dtype=np.float64)
            
            # The camera matrix
            focal_length = 1 * w
            cam_matrix = np.array([[focal_length, 0, w / 2],
                                [0, focal_length, h / 2],
                                [0, 0, 1]])

            # The Distance Matrix
            dist_matrix = np.zeros((4, 1), dtype=np.float64)

            # Use solvePnP function to get rotation vector
            success, rotation_vec, transition_vec = cv2.solvePnP(
                face_coordinate_in_real_world, face_coordinate_in_image,
                cam_matrix, dist_matrix)

            # Use Rodrigues function to convert rotation vector to matrix
            rotation_matrix, jacobian = cv2.Rodrigues(rotation_vec)

            result = rotation_matrix_to_angles(rotation_matrix)

            for i, info in enumerate(zip(('pitch', 'yaw', 'roll'), result)):
                k, v = info
                v = int(v)
                text = f'{k}: {int(v)}'
                # text = ''
                if k == 'pitch' : 
                    pitch = v
                if k == 'yaw' : 
                    yaw = v
                if k == 'roll':
                    roll = v
                # cv2.putText(image, text, (2, i * 30 + 50), cv2.FONT_HERSHEY_SIMPLEX, 1.2, (0, 0, 200), 2)
            
            # Calc ear for eye blink detection
            left_ear = eye_aspect_ratio(results.face_landmarks, LEFT_EYE)
            right_ear = eye_aspect_ratio(results.face_landmarks, RIGHT_EYE)
            ear = (left_ear + right_ear) /2

            # Calc distance between chin and sholder line for forward head posture
            sholderY = (results.pose_landmarks.landmark[11].y * h + results.pose_landmarks.landmark[12].y * h) / 2
            chinY = getLandmarksCoordinate(image, results.face_landmarks.landmark, 199)[1]
            csDist = sholderY - chinY
            
            text = f'CSdistance: {csDist:.1f}'
            cv2.putText(image, text, (2, 15 + 20), cv2.FONT_HERSHEY_SIMPLEX, 1.2, (0, 200, 0), 2)

            text = f'EAR: {ear:.3f}'
            cv2.putText(image, text, (2, 50+ 20), cv2.FONT_HERSHEY_SIMPLEX, 1.2, (0, 0, 200), 2)
           

            if ear < 0.34 :
                COUNTER +=1

            else:
                if COUNTER > 2:
                    TOTAL_BLINKS +=1
                    COUNTER =0

            # Draw landmark annotation on the image.
            image.flags.writeable = True 
            mp_drawing.draw_landmarks(
                image,
                results.face_landmarks,
                mp_holistic.FACEMESH_CONTOURS,
                landmark_drawing_spec=None,
                # connection_drawing_spec=mp_drawing_styles
                # .get_default_face_mesh_contours_style()
            )

            mp_drawing.draw_landmarks(
                image,
                results.pose_landmarks,
                mp_holistic.POSE_CONNECTIONS,
                # landmark_drawing_spec=mp_drawing_styles
                # .get_default_pose_landmarks_style()
            )
            
            #cv2.putText(image, f'Total Blinks: {TOTAL_BLINKS}',(10, 30),  cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
            
            points = []
            for i in (35, 168, 192, 282, 52, 416):
                points.append(getLandmarksCoordinate(image, results.face_landmarks.landmark, i))
            
            # Object pos & scale data for sending to unity
            posX = int(points[0][0])
            posY = int(points[1][1])
            sizeHead = int((euclaideanDistance(points[2],points[3]) + euclaideanDistance(points[4],points[5])) / 2)
            # send to Unity by stream output
            print(f'FromPython {posX} {posY} {sizeHead} {roll} {yaw} {pitch} {ear:.3f} {csDist:.1f}')
            # ear_list.append(ear)
            # csDist_list.append(csDist)
            if len(ear_list) > 300:
                del ear_list[0]
            sys.stdout.flush()

        # Flip the image horizontally for a selfie-view display.
        #cv2.imshow('MediaPipe Holistic', image)
       
        if cv2.waitKey(5) & 0xFF == 27:
            break

# import matplotlib.pyplot as plt
# # plt.plot(ear_list)
# # plt.show()

# # plt.plot(csDist_list)
# # plt.show()
cap.release()

# Send Data
# posX = coord[356][0]
# posY = coord[168][1]
# sizeHead = int((euclaideanDistance(coord[192],coord[282]) + euclaideanDistance(coord[52],coord[416])) / 2)
# roll = (coord[145][1] - coord[374][1])
# yaw = euclaideanDistance(coord[195],coord[130]) - euclaideanDistance(coord[195],coord[359])
# pitch = int(((coord[145][1] - coord[5][1]) + ( coord[374][1] - coord[5][1] )) / 2)

#Position
# index[6].x, y 미간
# 범위_ x: 0 ~ 640, y: 0 ~ 480

#Head scale 
# (([152].y - [10].y) + ([447].x - [227].x)) / 2
# ((coord[152][1] - coord[10][1]) + (coord[447][0] - coord[227][0])) / 2
# 범위_ 70 ~ 370


#Turn
# roll  (z턴)       눈의 y 값 차이
#                   [145].y - [374].y
# 범위_ x: -100 ~ 100

# yaw   (좌우)      코와 눈 거리 차이
#                   euclid([195], [130]) - euclid([359], [195])
# 범위_ x: -55 ~ 55

# pitch (위아래)    코와 눈 y 값 차이
#                   ([145].y - [5].y) + ([374]y - [5].y)
# (mesh_coordinatess[5][1] - mesh_coordinatess[145][1]) + (mesh_coordinatess[5][1] - mesh_coordinatess[374][1])) / 2
# 범위_ x: 0 ~ 50



# Get landmarks coordinates according to image scale
# eyes_ratio = blinkRatio(coord, RIGHT_EYE, LEFT_EYE)

# if eyes_ratio > 3.3:
#      COUNTER +=1

# else:
#     if COUNTER > 2:
#          TOTAL_BLINKS +=1
#          COUNTER =0