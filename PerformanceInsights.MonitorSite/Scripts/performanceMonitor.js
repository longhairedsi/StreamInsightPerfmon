/// <reference path="../scripts/jquery-1.7.1.js" />
/// <reference path="../scripts/jquery.signalR-1.0.0-rc1.js" />



$(function () {

    var stream = $.connection.PreformanceStream, // the generated client-side hub proxy
        $output = $('#output');
      
    /*function init() {
        return ticker.server.getAllStocks().done(function (CounterReading) {
            $output.empty();
            $output.empty();
            $.each(CounterReading, function () {
                //var stock = formatStock(this);
                $output.append(CounterReading.);
                //$output.append(liTemplate.supplant(stock));
            });
        });
    }*/

    // Add client-side hub methods that the server will call
    $.extend(stream.client, {
        updatePreformanceReading: function (counterReading) {
            $output.append(counterReading.payload.Value);
        },

        /*marketOpened: function () {
            $("#open").prop("disabled", true);
            $("#close").prop("disabled", false);
            $("#reset").prop("disabled", true);
            scrollTicker();
        },

        marketClosed: function () {
            $("#open").prop("disabled", false);
            $("#close").prop("disabled", true);
            $("#reset").prop("disabled", false);
            stopTicker();
        },

        marketReset: function () {
            return init();
        }*/
    });

    // Start the connection
    $.connection.hub.start()
        .pipe(init)
        .pipe(function () {
            return ticker.server.getMarketState();
        })
        .done(function (state) {
            if (state === 'Open') {
                ticker.client.marketOpened();
            } else {
                ticker.client.marketClosed();
            }

            // Wire up the buttons
            $("#open").click(function () {
                ticker.server.openMarket();
            });

            $("#close").click(function () {
                ticker.server.closeMarket();
            });

            $("#reset").click(function () {
                ticker.server.reset();
            });
        });
});