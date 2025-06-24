'use strict';
(function () {
  var TITLE_LENGTH = {
    MIN: 1,
    MAX: 100
  };

  var FORM_SELECTOR = {
    FORM: '.main-form',
    TITLE: '#title',
  };

  var form = document.querySelector(FORM_SELECTOR.FORM);
  var titleInput = form.querySelector(FORM_SELECTOR.TITLE);

  function onTitleInput(evt) {
    var valueLength = evt.target.value.length;

    if (valueLength < TITLE_LENGTH.MIN) {
      titleInput.setCustomValidity('Ещё ' + (TITLE_LENGTH.MIN - valueLength) + ' симв.');
    } else if (valueLength > TITLE_LENGTH.MAX) {
      titleInput.setCustomValidity('Ваш заголовок больше рекомендуемого на ' + (valueLength - TITLE_LENGTH.MAX) + ' симв.');
    } else {
      titleInput.setCustomValidity('');
    }
  }

  titleInput.addEventListener('input', onTitleInput);

  window.form = {
    element: form
  };
})();