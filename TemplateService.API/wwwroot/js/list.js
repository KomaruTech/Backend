'use strict';
(function () {

  var resultSection = document.querySelector('.result-section');
  var resultContainer = resultSection.querySelector('.result-wrapper');

  function appendElement(element, fragmentElement) {
    return fragmentElement.appendChild(element);
  }

  function createItem(data) {
    var template = document.querySelector('#template').content.querySelector('.result');
    var element = template.cloneNode(true);

    element.querySelector('.id').textContent = data.id;
    element.querySelector('.name').textContent = data.name;
    element.querySelector('.date').textContent = new Date(data.creationDate).toLocaleString();

    return element;
  }

  function renderItem(array) {
    var fragment = document.createDocumentFragment();

    array.forEach(function (arr) {
      if (arr.metas) {
        var itemElement = createItem(arr);
        appendElement(itemElement, fragment);
      }
    });

    appendElement(fragment, resultContainer);
  }

  function removeItems() {
    var items = Array.from(resultSection.querySelectorAll('.result'));

    items.forEach(function (it) {
      it.remove();
    });
  }

  window.list = {
    remove: removeItems,
    render: renderItem
  };
})();