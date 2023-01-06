/*  NOTE: This version is out of date! Please see Blackjack v.3 at:
    https://codepen.io/Clowerweb/pen/FDcxe  */
/*****************************************************************/
/*************************** Globals *****************************/
/*****************************************************************/

var game = new Game(),
    player = new Player(),
    dealer = new Player(),
    running = false,
    insured = 0,
    deal;

/*****************************************************************/
/*************************** Classes *****************************/

/*****************************************************************/

function Player() {
    var money = 0;
    if (localStorage.getItem("RouletteKey") == null) {
        money = parseInt(document.getElementById("hiddenCash").value);
    }else {
        var someVarName = localStorage.getItem("RouletteKey");
        money = parseInt(someVarName);
    }
    var hand = [],
        wager = 0,
        cash = money,
        bank = 0,
        ele = '',
        score = '';

    this.getElements = function () {
        if (this === player) {
            ele = '#phand';
            score = '#pscore span';
        } else if (this === dealer) {
            ele = '#dhand';
            score = '#dscore span';
        }

        return {'ele': ele, 'score': score};
    }

    this.getHand = function () {
        return hand;
    };

    this.setHand = function (card) {
        return hand.push(card);
    };

    this.resetHand = function () {
        return hand = [];
    };

    this.getWager = function () {
        return wager;
    };

    this.setWager = function (money) {
        return wager += parseInt(money);
    };

    this.resetWager = function () {
        return wager = 0;
    };

    this.checkWager = function () {
        return wager <= cash ? true : false;
    };

    this.getCash = function () {
        return cash;
    };

    this.setCash = function (money) {
        cash += money
        return this.updateBoard();
    };

    this.getBank = function () {
        if (bank >= 0) {
            return $('#bank').html('<strong>Winnings: $' + bank + '</strong>');
        } else {
            return $('#bank').html('<strong style="color: #C00">Winnings: -$' +
                bank.toString().replace('-', '') + '</strong>');
        }
    };

    this.setBank = function (money) {
        bank += money
        return this.updateBoard();
    };

    this.flipCards = function () {
        $('.down').each(function () {
            $(this).removeClass('down').addClass('up');
            renderCard(false, false, false, $(this));
        });
    }
}

function Deck() {
    var ranks = ['A', '2', '3', '4', '5', '6', '7', '8', '9', '10', 'J', 'Q', 'K'],
        suits = ['&#9824;', '&#9827;', '&#9829;', '&#9670;'],
        deck = [];

    this.getDeck = function () {
        return this.setDeck();
    };

    this.setDeck = function () {
        for (var i = 0; i < ranks.length; i++) {
            for (var x = 0; x < suits.length; x++) {
                var card = new Card({'rank': ranks[i]});

                deck.push({
                    'rank': ranks[i],
                    'suit': suits[x],
                    'value': card.getValue()
                });
            }
        }

        return deck;
    };
}

function Shuffle(deck) {
    var set = deck.getDeck(),
        shuffled = [];

    this.setShuffle = function () {
        while (set.length > 0) {
            var card = Math.floor(Math.random() * set.length);

            shuffled.push(set[card]);
            set.splice(card, 1);
        }

        return shuffled;
    };

    this.getShuffle = function () {
        return this.setShuffle();
    };
}

function Card(card) {
    this.getRank = function () {
        return card.rank;
    };

    this.getSuit = function () {
        return card.suit;
    };

    this.getValue = function () {
        var rank = this.getRank(),
            value = 0;

        if (rank === 'A') {
            value = 11;
        } else if (rank === 'K') {
            value = 10;
        } else if (rank === 'Q') {
            value = 10;
        } else if (rank === 'J') {
            value = 10;
        } else {
            value = parseInt(rank);
        }

        return value;
    };
}

function Deal(num) {
    var deck = new Deck(),
        shuffle = new Shuffle(deck),
        shuffled = shuffle.getShuffle(),
        card;

    this.getCard = function (sender) {
        this.setCard(sender);
        return card;
    };

    this.setCard = function (sender) {
        card = shuffled[0];
        shuffled.splice(card, 1);
        return sender.setHand(card);
    };

    this.dealCard = function (num, i, obj) {
        if (i >= num) return false;

        var sender = obj[i],
            elements = obj[i].getElements()
        score = elements.score,
            ele = elements.ele,
            dhand = dealer.getHand();

        deal.getCard(sender);

        if (i < 3) {
            renderCard(ele, sender, 'up');
            $(score).html(sender.getScore());
        } else {
            renderCard(ele, sender, 'down');
        }

        if (player.getHand().length < 3) {
            if (dhand.length > 0 && dhand[0].rank === 'A') {
                setActions('insurance');
            }

            if (player.getScore() === 21) {
                getWinner();
            } else {
                setActions('run');
            }
        }

        var showCards = function () {
            setTimeout(function () {
                deal.dealCard(num, i + 1, obj);
            }, 200);
        }

        clearTimeout(showCards());
    }
}

function Game() {
    this.newGame = function () {
        var wager = $('#wager').val().trim();

        player.resetWager();
        player.setWager(wager);

        if (player.checkWager()) {
            $('#deal').prop('disabled', true);
            resetBoard();
            player.setCash(-wager);

            deal = new Deal();
            running = true;
            insured = false;

            player.resetHand();
            dealer.resetHand();
            showBoard();
        } else {
            player.setWager(-wager);
            showAlert('Wager cannot exceed available cash!');
        }

        return false;
    };
}

/*****************************************************************/
/************************* Extensions ****************************/
/*****************************************************************/

Player.prototype.hit = function (dbl) {
    deal.dealCard(1, 0, [this]);

    var pscore = player.getScore(),
        hand = this.getHand();

    if (dbl || pscore > 21) {
        this.stand();
    } else {
        this.getHand();
    }

    setActions();

    return player.updateBoard();
}

Player.prototype.stand = function () {
    while (dealer.getScore() < 17 && player.getScore() <= 21) {
        dealer.flipCards();
        dealer.hit();
    }

    return getWinner();
}

Player.prototype.dbl = function () {
    var wager = this.getWager();

    if (this.checkWager(wager * 2)) {
        $('#double').prop('disabled', true);
        this.setWager(wager);
        this.setCash(-wager);

        return this.hit(true);
    } else {
        return showAlert('You don\'t have enough cash to double down!');
    }
}

Player.prototype.split = function () {
    showAlert('Split function is not yet working.');
}

Player.prototype.insure = function () {
    var wager = this.getWager() / 2,
        newWager = 0;

    $('#insurance').prop('disabled', true);
    this.setWager(wager);

    if (this.checkWager()) {
        newWager -= wager;
        this.setCash(newWager);
        insured = wager;
    } else {
        this.setWager(-wager);
        showAlert('You don\'t have enough for insurance!');
    }

    return false;
}

Player.prototype.getScore = function () {
    var hand = this.getHand(),
        score = 0,
        aces = 0;

    for (var i = 0; i < hand.length; i++) {
        score += hand[i].value;

        hand[i].value === 11 ? aces += 1 : false;

        if (score > 21 && aces > 0) {
            score -= 10;
            aces--;
        }
    }

    return score;
}

Player.prototype.updateBoard = function () {
    var score,
        cards;

    if (this === player) {
        score = '#pscore span',
            cards = '#phand';
    } else {
        score = '#dscore span',
            cards = '#dhand';
    }

    $(score).html(this.getScore());
    $('#cash span').html(player.getCash());
    var someVarName = player.getCash().toString();
    localStorage.setItem("RouletteKey", someVarName);
    player.getBank();

    return false;
}

/*****************************************************************/
/************************** Functions ****************************/

/*****************************************************************/

function numOnly(input) {
    input.on('keydown', function (e) {
        if (e.keyCode === 46 || e.keyCode === 8 || e.keyCode === 9 || e.keyCode === 27 || e.keyCode === 13 || (e.keyCode === 65 && e.ctrlKey === true) || (e.keyCode >= 35 && e.keyCode <= 39)) {
            return true;
        } else {
            if (e.shifKey || (e.keyCode < 48 || e.keyCode > 57) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        }
    });
}

function showAlert(msg) {
    return $('#alert span').html(msg).fadeIn().delay(3000).fadeOut();
}

function setActions(opts) {
    var hand = player.getHand();

    if (!running) {
        $('#deal').prop('disabled', false);
        $('#hit').prop('disabled', true);
        $('#stand').prop('disabled', true);
        $('#double').prop('disabled', true);
        $('#split').prop('disabled', true);
        $('#insurance').prop('disabled', true);
    }

    if (opts === 'run') {
        $('#deal').prop('disabled', true);
        $('#hit').prop('disabled', false);
        $('#stand').prop('disabled', false);

        if (player.checkWager(wager * 2)) {
            $('#double').prop('disabled', false);
        }
    } else if (opts === 'split') {
        $('#split').prop('disabled', false);
    } else if (opts === 'insurance') {
        $('#insurance').prop('disabled', false);
    } else if (hand.length > 2) {
        $('#double').prop('disabled', true);
        $('#split').prop('disabled', true);
        $('#insurance').prop('disabled', true);
    }

    return false;
}

function showBoard() {
    return deal.dealCard(4, 0, [player, dealer, player, dealer]);
}

function renderCard(ele, sender, type, item) {
    var hand, i, card;

    if (!item) {
        hand = sender.getHand(),
            i = hand.length - 1,
            card = new Card(hand[i]);
    } else {
        hand = dealer.getHand(),
            card = new Card(hand[1]);
    }

    var rank = card.getRank(),
        suit = card.getSuit(),
        color = 'red',
        posx = 0,
        posy = '60px',
        speed = 200,
        cards = ele + ' .card-' + i;

    if (ele === '#phand') {
        posy = '230px';
        speed = 500;
    }

    if (!item) {
        $(ele).append('<div class="card-' + i + ' ' + type + '">&nbsp;</div>');
    } else {
        cards = item;
    }

    if (type === 'up' || item) {
        if (suit !== '&#9829;' && suit !== '&#9670;') {
            color = 'black';
        }

        $(cards).html(
            '<span class="' + color + ' pos-0">' +
            '<span class="rank">' + rank + '</span>' +
            '<span class="suit">' + suit + '</span>' +
            '</span>' +
            '<span class="' + color + ' pos-1">' +
            '<span class="rank">' + rank + '</span>' +
            '<span class="suit">' + suit + '</span>' +
            '</span>'
        );
    }

    if (!item) {
        $(ele + ' .card-' + i).animate({'top': posy, 'right': $(ele).width() / 2}, speed);
    }

    return false;
}

function resetBoard() {
    $('#dscore span').html('');
    $('#pscore span').html('');
    $('#dhand').html('');
    $('#phand').html('');
    $('#result').html('');
}

function getWinner() {
    var phand = player.getHand(),
        dhand = dealer.getHand(),
        pscore = player.getScore(),
        dscore = dealer.getScore(),
        wager = player.getWager(),
        cash = player.getCash(),
        winnings = 0;

    running = false;
    setActions();

    if (pscore > dscore) {
        if (pscore === 21 && phand.length < 3) {
            winnings = (wager * 2) + (wager / 2);
            player.setCash(winnings);
            player.setBank(winnings - wager);
            $('#result').html('Blackjack!');
        } else if (pscore <= 21) {
            winnings = wager * 2;
            player.setCash(winnings);
            player.setBank(winnings - wager);
            $('#result').html('You win!');
        } else if (pscore > 21) {
            winnings -= wager;
            player.setBank(winnings);
            $('#result').html('Bust');
        }
    } else if (pscore < dscore) {
        if (pscore <= 21 && dscore > 21) {
            winnings = wager * 2;
            player.setCash(winnings);
            player.setBank(winnings - wager);
            $('#result').html('You win - dealer bust!');
        } else if (dscore <= 21) {
            winnings -= wager;
            player.setBank(winnings);
            $('#result').html('You lose!');
        }
    } else if (pscore === dscore) {
        if (pscore <= 21) {
            winnings = wager;
            player.setCash(winnings);
            $('#result').html('Push');
        } else {
            winnings -= wager;
            player.setBank(winnings);
            $('#result').html('Bust');
        }
    }
    dealer.flipCards();
    dealer.updateBoard();

    if (cash <= 0) {
        var newGame = confirm('You\'re broke! Would you like to play again?');

        if (newGame) {
            player.setCash(1000);
        }
    }


}

/*****************************************************************/
/*************************** Actions *****************************/
/*****************************************************************/

$('#deal').on('click', function () {
    var cash = player.getCash();

    if (cash > 0 && !running) {
        if ($('#wager').val().trim() > 0) {
            game.newGame();
        } else {
            showAlert('Wager must be at least $1!');
        }
    } else {
        var newGame = confirm('You\'re broke! Would you like to play again?');

        if (newGame) {
            player.setCash(1000);
        }
    }
});

$('#hit').on('click', function () {
    player.hit();
});

$('#stand').on('click', function () {
    player.stand();
});

$('#double').on('click', function () {
    player.dbl();
});

$('#split').on('click', function () {
    player.split();
});

$('#insurance').on('click', function () {
    player.insure();
});

/*****************************************************************/
/*************************** Loading *****************************/
/*****************************************************************/

numOnly($('#wager'));
$('#wager').val(100);
$('#cash span').html(player.getCash());
player.getBank();

var btn = document.getElementById("btn-for-update-asset");

btn.addEventListener("click", function() {
  document.getElementById("LastAsset").value = player.getCash();
});
if (window.location.href == "https://localhost:5001/home/blackjack"){
    let blackjackViewModel = {
        Cash: 20.0,
        LastAsset: parseInt(localStorage.getItem("RouletteKey"))
    };
    var data = JSON.stringify(blackjackViewModel);
    console.log(data)
    $.ajax({
        type: 'POST',
        url: '/Home/UpdateBlackjackWithRefresh',
        contentType: 'application/json', // when we use .serialize() this generates the data in query string format. this needs the default contentType (default content type is: contentType: 'application/x-www-form-urlencoded; charset=UTF-8') so it is optional, you can remove it
        data: data,
        success: function (result) {
            alert('Welcome To The BLACKJACK in 42XBET ');
            console.log(result);
        },
        error: function () {
            alert('Failed to receive the Data');
            console.log('Failed ');
        }
    })
    console.log("window loaded");
}