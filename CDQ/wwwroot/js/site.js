// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function removeDot(numberText) {
    while (numberText.indexOf(".") >= 0) {
        numberText = numberText.replace(".", "");
    };
    return numberText;
}

function formatNumber(numberText) {
    if (numberText == "") return "0";

    return numberText;
}


function formatMoney(numberText) {
    if (numberText == "" || numberText == "," || numberText =="-") return "0,00";

    while (numberText.indexOf(".") >= 0) {
        numberText = numberText.replace(".", "");
    };
    numberText = numberText.replace(",", ".");
    num = parseFloat(numberText);
    return (
        num
            .toFixed(2) 
            .replace('.', ',') 
            .replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.') 
    ) // use . as a separator
}

function handleReplaceDotWithComma(sender, e) {
    var k = e.key;
    //console.log('Key:', k);
    var objInput = sender;
    var s = objInput.value;
    if (k == 'Tab' || k === 'Backspace' || k === 'Delete' || k === 'Left' || k === 'Right' || k === 'Home' || k === 'End' || k === 'ArrowLeft' || k === 'ArrowRight') {
        //gestisco i tasti di movimento
        return true;
    }
    if (s.indexOf(',') >= 0 && (k === '.' || k === 'Decimal' || k === ',')) {
        // impedisco che vengano inserite più virgole
        return false;
    }

    if (s.length > 0 && k === '-') {
        //Accetto solo un segno - all'inizio

        return false;
    }

    if (k === '.' || k === 'Decimal') {
        // sostituisco il punto con la virgola (anche con il tastierino numerico)

        if (document.selection) {
            //IE
            var range = document.selection.createRange();
            range.text = ',';
        } else if (objInput.selectionStart || objInput.selectionStart == '0') {
            // Chrome + FF
            var start = objInput.selectionStart;
            var end = objInput.selectionEnd;
            objInput.value = s.substring(0, start) + ',' + s.substring(end, s.length);
            objInput.selectionStart = start + 1;
            objInput.selectionEnd = start + 1;
        } else {
            objInput.value = s + ',';
        }
        return false;
    }
    if ('1234567890-,'.indexOf(k) == -1) {
        return false;
    }
    return true;
}

function handleReplaceNotNumberWithNothing(sender, e) {
    var k = e.key;
    //console.log('Key:', k);
    var objInput = sender;
    var s = objInput.value;
    if (k == 'Tab' || k === 'Backspace' || k === 'Delete' || k === 'Left' || k === 'Right' || k === 'Home' || k === 'End' || k === 'ArrowLeft' || k === 'ArrowRight') {
        //gestisco i tasti di movimento
        return true;
    }    
    if ('1234567890'.indexOf(k) == -1) {
        return false;
    }
    return true;
}
