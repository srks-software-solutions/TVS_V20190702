$(document).ready(function () {
    var data1 = '';
    GetChart1();
    GetChart2();
      setInterval(GetChart1, 60 * 1000);
      setInterval(GetChart2, 1000);

      setInterval(machines, 5000);
    var chart1 = AmCharts.makeChart("chartdiv",
                {
                    "type": "serial",
                    "categoryField": "MachineName",
                    "angle": 30,
                    "depth3D": 30,
                    "startDuration": 1,
                    "categoryAxis": {
                        "gridPosition": "start"
                    },
                    "trendLines": [],
                    "graphs": [
                          {
                              "balloonText": "[[title]] of [[MachineName]]:[[value]]  hrs",
                              "fillAlphas": 1,
                              "id": "AmGraph-1",
                              "title": "Running Time",
                              "type": "column",
                              "valueField": "RunningTime",
                              "fillColors": "#008800",
                              "color": "#000000",
                              "lineAlpha": 0.3

                          },

                        {
                            "balloonText": "[[title]] of [[MachineName]]:[[value]]  hrs",
                            "fillAlphas": 1,
                            "id": "AmGraph-2",
                            "title": "Idle Time",
                            "type": "column",
                            "valueField": "IdleTime",
                            "fillColors": "#FFFF00",
                            "color": "#000000",
                            "lineAlpha": 0.3

                        },
                       {
                           "balloonText": "[[title]] of [[MachineName]]:[[value]] hrs",
                           "fillAlphas": 0.8,
                           "id": "AmGraph-2",
                           "title": "Breakdown Time",
                           "type": "column",
                           "valueField": "BreakDownTime",
                           "fillColors": "#FF0000",
                           "color": "#000000",
                           "lineAlpha": 0.3


                       },

                        {
                            "balloonText": "[[title]] of [[MachineName]]:[[value]]  hrs",
                            "fillAlphas": 1,
                            "id": "AmGraph-4",
                            "title": "Power Off",
                            "type": "column",
                            "valueField": "PowerOffTime",
                            "fillColors": "#000088",
                            "color": "#000000",
                            "lineAlpha": 0.3

                        }
                    ],
                    "guides": [],
                    "valueAxes": [
                        {

                            "id": "ValueAxis-1",
                            "stackType": "3d",
                            "title": "Time in (Hrs)",
                            "minimum": 0,
                            "maximum": 24,
                            "autoGridCount": false,
                            "gridCount": 100,
                            "labelFrequency": 4,

                        }
                    ],
                    "allLabels": [],
                    "balloon": {},
                    "legend": {
                        "enabled": true,
                        "useGraphSettings": true
                    }

                }
        );

    function machines() {
        if (data1 != '') {
            window.location.href = "/MConnextDashboard/Index";
        }
    }
    var chart = AmCharts.makeChart("chartdiv1", {
        "type": "serial",
        "theme": "light",
        "marginTop": 0,
        "marginRight": 0,

        "valueAxes": [{
            "minimum": -0.5,
            "maximum": 1.5,
            "autoGridCount": false,
            "gridCount": 100,
            "axisAlpha": 0,
            "position": "left",
            "guides": [{
                "value": 1.1,
                "lineAlpha": 1,
                "lineColor": "#880000"
            }, {
                "value": -0.1,
                "lineAlpha": 1,
                "lineColor": "#880088"
            }],
            "zeroGridAlpha": 0
        }],
        "graphs": [{
            "balloonText": "<div style='margin:5px; font-size:19px;'><span style='font-size:13px;'>[[category]]</span><br>[[value]]</div>",
            "bullet": "round",
            "bulletSize": 8,
            "bulletBorderAlpha": 0,
            "lineThickness": 2,
            "negativeLineColor": "#FFC107",
            "negativeBase": 0.8,
            "type": "smoothedLine",
            "valueField": "value"
        }, {
            "showBalloon": false,
            "bullet": "round",
            "bulletBorderAlpha": 0,
            "hideBulletsCount": 50,
            "lineColor": "transparent",
            "negativeLineColor": "#D50000",
            "negativeBase": -0.05,
            "type": "smoothedLine",
            "valueField": "value"
        }],
        "chartScrollbar": {
            "graph": "g1",
            "gridAlpha": 0,
            "color": "#888888",
            "scrollbarHeight": 55,
            "backgroundAlpha": 0,
            "selectedBackgroundAlpha": 0.1,
            "selectedBackgroundColor": "#888888",
            "graphFillAlpha": 0,
            "autoGridCount": true,
            "selectedGraphFillAlpha": 0,
            "graphLineAlpha": 0.2,
            "graphLineColor": "#c2c2c2",
            "selectedGraphLineColor": "#888888",
            "selectedGraphLineAlpha": 1

        },
        "chartCursor": {
            "categoryBalloonDateFormat": "JJ:NN:SS",
            "cursorAlpha": 0,
            "valueLineEnabled": false,
            "valueLineBalloonEnabled": false,
            "valueLineAlpha": 0.5,
            "fullWidth": true
        },
        "dataDateFormat": "HH:NN",
        "categoryField": "Time",
        "categoryAxis": {
            "minPeriod": "mm",
            "parseDates": true,
            "minorGridAlpha": 0.1,
            "minorGridEnabled": true
        }
    });

    chart.addListener("rendered", zoomChart);
    if (chart.zoomChart) {
        chart.zoomChart();
    }

    function zoomChart() {
        chart.zoomToIndexes(Math.round(chart.dataProvider.length * 0.4), Math.round(chart.dataProvider.length * 0.5));
    }

    function GetChart1() {
        $.get('/MConnextDashboard/GetAxisDetails', {}, function (msg) {
            data1 = msg;
            if (msg !== '') {
                chart1.dataProvider = JSON.parse(msg);
                chart1.validateData();
            }
        });
    }



    function GetChart2() {
        $.get('/MConnextDashboard/GetSpindleLoad', {}, function (msg) {
            data1 = msg;
            if (msg !== '') {
                chart.dataProvider = JSON.parse(msg);
                chart.validateData();
            }
        });
    }
});
