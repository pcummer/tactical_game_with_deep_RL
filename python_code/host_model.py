# import the necessary packages
from tensorflow import keras
from PIL import Image
import numpy as np
import flask
import io
import os
from datetime import datetime

# initialize our Flask application and the Keras model
app = flask.Flask(__name__)
model = None
feature_depth = 10
total_vision_length = 13
random_threshold = 0.8

training_directory = 'training_data/'
discount = 0.99
batch_size = 128
data_full = np.array([], dtype=object).reshape(0,3)


def load_model():
    # load the pre-trained Keras model (here we are using a model
    # pre-trained on ImageNet and provided by Keras, but you can
    # substitute in your own networks just as easily)
    global model
    model = keras.models.load_model('model_v2.h5')


@app.route("/predict", methods=["POST"])
def predict():
    # initialize the data dictionary that will be returned from the
    # view
    # data = {"success": False}

    # ensure an image was properly uploaded to our endpoint
    if flask.request.method == "POST":
        if flask.request.form:
            # read the image in PIL format
            data = flask.request.form

            data = np.asarray([value for value in data.to_dict().values()])

            data_formatted = np.array([], dtype=object).reshape(0,feature_depth)
            for i in range(0, round(len(data)/feature_depth)): # we want this to fail if it's not an int
                row = data[i*feature_depth:(i+1)*feature_depth]
                data_formatted = np.concatenate((data_formatted, np.expand_dims(row, axis=0)))

            data_formatted_3D = np.array([], dtype=object).reshape(0,total_vision_length,feature_depth)
            for i in range(0, round(len(data_formatted)/total_vision_length)):
                row = data_formatted[i * total_vision_length:(i + 1) * total_vision_length]
                data_formatted_3D = np.concatenate((data_formatted_3D, np.expand_dims(row, axis=0)))

            data_formatted_3D = data_formatted_3D.astype(float)
            prediction = model.predict(np.expand_dims(data_formatted_3D, axis=0))
            output = np.argmax(prediction)
            if np.random.rand(1) > random_threshold or output == 0:
                output = np.random.randint(0, 5, 1)[0]


            # indicate that the request was a success
            #data["success"] = True

    # return the data dictionary as a JSON response
    return str(output)

@app.route("/save", methods=["POST"])
def save():
    # initialize the data dictionary that will be returned from the
    # view
    # data = {"success": False}
    global data_full

    # ensure an image was properly uploaded to our endpoint
    if flask.request.method == "POST":
        if flask.request.form:
            # read the image in PIL format
            data = flask.request.form

            data = np.asarray([value for value in data.to_dict().values()])
            action = data[0]
            reward = data[1]
            data = data[2:]

            data_formatted = np.array([], dtype=object).reshape(0,feature_depth)
            for i in range(0, round(len(data)/feature_depth)): # we want this to fail if it's not an int
                row = data[i*feature_depth:(i+1)*feature_depth]
                data_formatted = np.concatenate((data_formatted, np.expand_dims(row, axis=0)))

            data_formatted_3D = np.array([], dtype=object).reshape(0,total_vision_length,feature_depth)
            for i in range(0, round(len(data_formatted)/total_vision_length)):
                row = data_formatted[i * total_vision_length:(i + 1) * total_vision_length]
                data_formatted_3D = np.concatenate((data_formatted_3D, np.expand_dims(row, axis=0)))

            data_formatted_3D = data_formatted_3D.astype(float)
            data_full = np.concatenate((data_full, np.expand_dims(np.array((data_formatted_3D, action, reward)),axis=0)),axis=0)
            np.save('training_data/' + str(action) + '_' + str(reward) + '_'+ str(datetime.now().timestamp()) + '.npy',data_formatted_3D)
    return 'done'


@app.route("/train", methods=["POST"])
def train():
    if flask.request.method == "POST":
        if flask.request.form:
            data = flask.request.form

            data = np.asarray([value for value in data.to_dict().values()])
            batches = data[0]
            batches = int(batches)
            model.compile(optimizer='adam', loss='mse')

            for j in range(0, batches):
                target = np.array([], dtype='float32').reshape(0, 5)
                features = np.array([], dtype='float32').reshape(0, total_vision_length, total_vision_length, feature_depth)
                for record in data_full[[x for x in np.random.randint(0,len(data_full),batch_size)]]:
                    data = record[0]
                    data_last = data[0:total_vision_length]
                    data_next = data[total_vision_length:]
                    action = int(record[1])
                    reward = float(record[2])
                    q_values_last = model.predict(np.expand_dims(data_last, axis=0))
                    q_values_next = model.predict(np.expand_dims(data_next, axis=0))
                    q_values_last[0, action] = reward + discount * np.max(q_values_next)
                    target = np.concatenate((target, q_values_last), axis=0)
                    features = np.concatenate((features, np.expand_dims(data_last, axis=0)))

                model.fit(features, target.astype(float), epochs=50)

            model.save('model_v2.h5')
    return 'done'


# if this is the main thread of execution first load the model and
# then start the server
if __name__ == "__main__":
    print(("* Loading Keras model and Flask starting server..."
        "please wait until server has fully started"))
    load_model()
    app.run()