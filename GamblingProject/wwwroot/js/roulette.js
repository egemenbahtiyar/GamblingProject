var number = 0;
var amount = 5;
var money =0;
if (localStorage.getItem("someVarKey") == null) {
    money = parseInt(document.getElementById("hiddenCash").value);
}else {
    var someVarName = localStorage.getItem("someVarKey");
    money = parseInt(someVarName);
}

// red green black
var bets = [0, 0, 0];

var cheating = false;

var time = 300;

var keys = [];

$(document).keydown(function (e) {
    keys[e.keyCode] = true;
    if (keys[67]) {
        // c
        cheating = true;
    }
    if (keys[78]) {
        // n
        cheating = false;
    }
});

$(document).keyup(function (e) {
    delete keys[e.keyCode];
});

for (var i = 0; i < 25; i++) {
    var num = random(0, 14);
    var color = "red";
    if (num % 2 == 0) color = "black";
    if (num == 0) color = "green";
    var element = `<div class="box ${color}" id="box${i}">${num}</div>`;
    $("#history").append(element);
}

function reset() {
    $("#roulette .box").remove();
    $("#result .box").remove();
    for (var i = 0; i < 500; i++) {
        number = random(0, 14);
        var color = "red";
        if (i % 2 == 0) color = "black";
        if (number == 0) color = "green";
        var element = `<div class="box ${color}" id="box${i}">${number}</div>`;
        number++;
        if (number > 14) {
            number = 0;
        }
        $("#roulette").append(element);
    }
}

reset();

function spin() {
    reset();
    $("#result").html("");
    var rand = random(1000, (500 - 16) * 50);

    if (cheating) {
        var childNumber = Math.floor(rand / 50) + 8;
        if (Math.random() < 1) {
            $("#box" + childNumber)
                .addClass("green")
                .removeClass("red")
                .removeClass("black")
                .html("0");
        }
    }
    $("#roulette .box")
        .first()
        .animate(
            {
                marginLeft: -rand
            },
            5000,
            "easeOutCubic",
            function () {
                var childNumber = Math.floor(rand / 50) + 8;
                var child = $("#box" + childNumber);
                checkBet(child);
                child
                    .clone(function () {
                        this.id = "won";
                    })
                    .appendTo("#result");

                child
                    .clone(function () {
                        this.id = "d" + Math.random();
                    })
                    .appendTo("#history");

                time = 200;
            }
        );
}

function random(min, max) {
    return Math.floor(Math.random() * (max - min) + min);
}

setInterval(function () {
    document.getElementById("money").innerHTML = "Balance: " + money.toLocaleString() + " $";
    var perc = (time / 200) * 800;
    document.getElementById("time").style.width = perc + "px";
    if (time > 0) {
        time--;
    }
    if (time == 0) {
        spin();
        time = -1;
    }
}, 1000 / 20);

function bet(color, btn) {
    var betSize = parseFloat(document.getElementById("betInput").value);
    if (money < betSize || !betSize) return;

    if (color == "r") {
        money -= betSize;
        bets[0] += betSize;
        var element = `<div class="betText"><span class="name">YOU</span>${betSize.toLocaleString()}</div>`;
        $(element).appendTo($(btn).parent()).slideUp(1).slideDown(1000);
    }
    if (color == "g") {
        money -= betSize;
        bets[1] += betSize;
        var element = `<div class="betText"><span class="name">YOU</span>${betSize.toLocaleString()}</div>`;
        $(element).appendTo($(btn).parent()).slideUp(1).slideDown(1000);
    }
    if (color == "b") {
        money -= betSize;
        bets[2] += betSize;
        var element = `<div class="betText"><span class="name">YOU</span>${betSize.toLocaleString()}</div>`;
        $(element).appendTo($(btn).parent()).slideUp(1).slideDown(1000);
    }
}

function checkBet(result) {
    var color = $(result).attr("class").replace(/box /, "");
    if (color == "green") {
        money += bets[1] * 14;
        if (bets[1] < 5000) return;
        var rand = random(2, 25);
    }
    if (color == "red") {
        money += bets[0] * 2;
    }
    if (color == "black") {
        money += bets[2] * 2;
    }
    document.getElementById("LastAsset").value = money;
    var someVarName = money.toString();
    localStorage.setItem("someVarKey", someVarName);
    localStorage.setItem("RouletteKey", someVarName);

    $(".betText").slideDown(1000, function () {
        $(this).remove();
    });

    document.getElementById("betInput").value = "";

    bets = [0, 0, 0];
}
var leaveButton = document.getElementById("btn-for-roulette-update-asset");
leaveButton.addEventListener("click", function() {
    document.getElementById("LastAsset").value = money;
});

if (window.location.href == "https://localhost:5001/home/roulette"){
    let rouletteViewModel = {
        Cash: 20.0,
        LastAsset: parseInt(localStorage.getItem("someVarKey"))
    };
    var data = JSON.stringify(rouletteViewModel);
    console.log(data)
    $.ajax({
        type: 'POST',
        url: '/Home/UpdateRouletteWithRefresh',
        contentType: 'application/json', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
        data: data,
        success: function (result) {
            alert('Successfully received Data ');
            console.log(result);
        },
        error: function () {
            alert('Failed to receive the Data');
            console.log('Failed ');
        }
    })
    console.log("window loaded");
}
