'use strict';
(function () {
    var OK_STATUS = 200;
    var CREATE_STATUS = 201;
  var TIMEOUT_MS = 5000;

  function createXhrRequest() {
    var xhr = new XMLHttpRequest();
    xhr.responseType = 'json';
    return xhr;
  }

  function isSuccessStatus(status) {
      return status === OK_STATUS || status === CREATE_STATUS;
  }

  function createXhr(onSuccess, onError) {
    var xhr = createXhrRequest();

    xhr.timeout = TIMEOUT_MS;

    xhr.addEventListener('load', function () {
      if (isSuccessStatus(xhr.status)) {
        onSuccess(xhr.response);
      } else {
        onError('Статус ответа: ' + xhr.status + ' ' + xhr.statusText);
      }
    });

    xhr.addEventListener('error', function () {
      onError('Произошла ошибка соединения');
    });

    xhr.addEventListener('timeout', function () {
      onError('Запрос не успел выполниться за ' + xhr.timeout + ' мс');
    });

    return xhr;
  }

  function getData(url, onSuccess, onError) {
    var xhr = createXhr(onSuccess, onError);
    xhr.open('GET', url);
    xhr.send();
  }

  function sendData(url, data, onSuccess, onError) {
    var xhr = createXhr(onSuccess, onError);
      xhr.open('POST', url);
      xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
    xhr.send(data);
  }

  window.data = {
    get: getData,
    send: sendData
  };
})();