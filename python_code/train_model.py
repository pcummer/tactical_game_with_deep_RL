from tensorflow import keras
import pandas as pd
import numpy as np
import math
import os

training_directory = 'training_data/'
discount = 0.99
batches = 5
batch_size = 1024

def zip_data(training_directory, batch_size, vision_length, feature_depth):
    files = os.listdir(training_directory)
    target = np.array([], dtype='float32').reshape(0,5)
    features = np.array([], dtype='float32').reshape(0, vision_length, vision_length, feature_depth)
    for file in files:
        data = np.load(training_directory+file,allow_pickle=True)
        data_last = data[0:13]
        data_next = data[13:]
        action = int(file.split('_')[0])
        reward = float(file.split('_')[1])


model = keras.models.load_model('model_v2.h5')
model.compile(optimizer='adam', loss='mse')

files = os.listdir(training_directory)
for j in range(0, batches):
    target = np.array([], dtype='float32').reshape(0,5)
    features = np.array([], dtype='float32').reshape(0, 13, 13, 10)
    for file in [files[x] for x in np.random.randint(0, len(files), batch_size)]:
        data = np.load(training_directory+file, allow_pickle=True)
        data_last = data[0:13]
        data_next = data[13:]
        action = int(file.split('_')[0])
        reward = float(file.split('_')[1])
        q_values_last = model.predict(np.expand_dims(data_last, axis=0))
        q_values_next = model.predict(np.expand_dims(data_next, axis=0))
        q_values_last[0,action] = reward + discount * np.max(q_values_next)
        target = np.concatenate((target,q_values_last), axis=0)
        features = np.concatenate((features, np.expand_dims(data_last, axis=0)))

    model.fit(features, target.astype(float), epochs = 50)

model.save('model_v2.h5')


