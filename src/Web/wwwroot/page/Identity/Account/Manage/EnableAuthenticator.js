$(document).ready(function () {
    var qrCodeData = document.getElementById('qrCodeData').dataset.url;
    QRCode.toCanvas(document.getElementById('qrCode'),
        qrCodeData,
        function (error) {
            if (error) console.error(error);
            //else console.log('qr painted');
        });
});