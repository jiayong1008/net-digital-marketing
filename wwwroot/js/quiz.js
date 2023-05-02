// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {

    populateAnswerList();

    // Add new option input field
    $('#add-option-button').click(function () {
        var optionCount = $('.option-input').length;
        var newOption = '<div class="input-group">' +
            '<input type="text" class="form-control option-input" name="QuestionOptions[' + optionCount + '].Option" asp-for="QuestionOptions[' + optionCount + '].Option" />' +
            '<div class="input-group-append">' +
            '<button type="button" class="btn btn-outline-secondary remove-option-button">Remove</button>' +
            '</div>' +
            '</div>';
        $('#options-container').append(newOption);
    });

    // Remove option input field
    $(document).on('click', '.remove-option-button', function () {
        var optionId = $(this).closest('.input-group').index();
        var answerId = $('#answer-dropdown').val();
        $(this).closest('.input-group').remove();
        $('#answer-dropdown').find('option[value="' + optionId + '"]').remove();

        // Update option values
        $('#answer-dropdown option:gt(0)').each(function (index) {
            $(this).val(index);
        });

        if (optionId == answerId) {
            $('#answer-dropdown').val('');
        }
    });

    // Update answer dropdown list
    $(document).on('change', '.option-input', function () {
        var optionId = $(this).closest('.input-group').index();
        var optionText = $(this).val();
        var answerDropdown = $('#answer-dropdown');
        var answerOption = '<option value="' + optionId + '">' + optionText + '</option>';

        // Remove previous answer option, if any
        answerDropdown.find('option[value="' + optionId + '"]').remove();

        // Add new answer option
        if (optionText) {
            answerDropdown.append(answerOption);
        }
    });

});

function populateAnswerList() {

    var answerDropdown = document.querySelector('#answer-dropdown');
    var length = answerDropdown.options.length;
    if (answerDropdown.options.length > 1) return;

    var inputs = document.querySelectorAll('#options-container input');
    var optionId = 0;

    inputs.forEach(option => {
        var optionText = option.value;

        if (optionText) {
            var answerOption = '<option value="' + optionId + '">' + optionText + '</option>';
            answerDropdown.append(answerOption);
        }
        optionId++;
    });
}