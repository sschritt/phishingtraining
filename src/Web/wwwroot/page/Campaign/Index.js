$(document).ready(function () {
    $('.input-joinlink').click(function (event) {
        event.target.select();
    });
    $('.button-copy').click(function (event) {
        var element = $(event.target).closest(".input-group").find(".input-joinlink");
        copyValueToClipboard(element);
    });
    $('.input-joinlinkraw').each(function (idx, element) {
        var destinationElement = $(element).siblings(".input-joinlink")[0];
        destinationElement.value = document.location.origin + element.value;
    });
});

function copyValueToClipboard(inputElement) {
    inputElement.select();

    document.execCommand("copy");
}