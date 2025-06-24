'use strict';
(function () {
  var URL = {
    GET: window.location.origin + '/api/v1/Document',
    POST: window.location.origin + '/api/v1/Document',
  };


  var form = window.form.element;
  var getData = window.data.get;
  var sendData = window.data.send;
  var renderItems = window.list.render;
  var removeItems = window.list.remove;

  window.templates = [];

  function onError(message) {
    console.log(message);
  }

  function onSuccess(data) {
    window.templates = data;
    renderItems(window.templates);
    }

    function onFormSuccess(data) {
        removeItems();
        window.templates.push(data);
        renderItems(window.templates);
    }

  function clearPage() {
    window.templates = [];
    removeItems();
    form.reset();
  }

  function onFormSubmit(evt) {
    evt.preventDefault();
    var body = {
        documentName: new FormData(form).get('title'),
      }
    sendData(URL.POST, JSON.stringify(body), onFormSuccess, onError);
      //clearPage();
  }

  getData(URL.GET, onSuccess, onError);
  form.addEventListener('submit', onFormSubmit);
})();