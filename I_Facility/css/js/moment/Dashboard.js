$(document).ready(function () {

    //$("#Dashboard2").css("display", "none");
    //$("#loading").css("display", "block");
    var MachineUTFDet = '';
    var SpindleLoadDet = '';
    pageload1();
    
    //GetChart2();
    var data1 = '',data2='';
    setInterval(pageload1, 1000);
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
                    "labelFrequency": 4
                }
            ],
            "allLabels": [],
            "balloon": {},
            "legend": {
                "enabled": true,
                "useGraphSettings": true
            },
            "titles": [
                {
                    "id": "Title-1",
                    "size": 15,
                    "text": "Utilization Report"
                }
            ]
        }
       );

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
        },

        "titles": [
{
    "id": "Title-1",
    "size": 15,
    "text": "Spindle Load Trends"
}
        ]
    });

    chart.addListener("rendered", zoomChart);
    if (chart.zoomChart) {
        chart.zoomChart();
    }

    function zoomChart() {
        chart.zoomToIndexes(Math.round(chart.dataProvider.length * 0.4), Math.round(chart.dataProvider.length * 0.5));
    }

    

    function swiping()
    {
        var swiper = new Swiper('.blog-slider', {
            spaceBetween: 30,
            effect: 'fade',
            loop: true,
            mousewheel: {
                invert: false,
            },
            autoplay: {
                delay: 20000,
            },
            autoHeight: true,
            pagination: {
                el: '.blog-slider__pagination',
                clickable: true,
            }
        });
    }


    function pageload1() {
        $.get("/MConnextDashboard/GetMachine", {}, function (msg) {


            $('.MACDET').empty();
            //$('.MACDET1').empty();
            if (msg !== '') {
                $("#loading").css("display", "none");
                var data = JSON.parse(msg);
                var cssdata = '';
                var cssdata1 = '';
                var c = 0;
                for (var i = 0; i < data.length ; i++) {
                    data1 = msg;
                    MachineUTFDet = data[i].MachineUTFs;
                    SpindleLoadDet = data[i].Spindleloads;
                    //var livecam = '';
                    var machineName = '';
                    var color = '';
                    if (data[i].Color === 'GREEN')
                        color = 'dash-green';
                    else if (data[i].Color === 'YELLOW')
                        color = 'dash-amber';
                    else if (data[i].Color === 'BLUE')
                        color = 'dash-blue';
                    else if (data[i].Color === 'RED')
                        color = 'dash-red';
                    var img = '';
                    if (i == 0) {
                        //img = "http://localhost:21812/images/TTB-20A.png";
                        machineName = data[i].MachineName + " with FANUC robot";
                        //livecam = Livecam;
                    }
                    else if (i == 1 || i == 2) {
                        if (i == 1) {
                            //livecam = livecam1;
                            machineName = data[i].MachineName + " with ABB robot";
                        }
                        else {
                            //ivecam = livecam2;
                            machineName = data[i].MachineName + " with Machine application";
                        }
                        //img = "http://localhost:21812/images/NXV560A.png";
                    }

                    else if (i === 3) {
                        machineName = 'ISSOKU measuring automation';
                        //img = "http://localhost:21812/images/Hp_Dsr2_K2.png";
                        //livecam = livecam3;
                    }
                    else {
                        machineName = 'Pushcorp Deburring and polishing automation';
                        //img = "http://localhost:21812/images/polish.png";
                        //livecam = livecam4;
                    }

                    if (i < 3) {
                        //cssdata += '<div class="row left-m-t40" style="border: 1px solid #fff;"><div class="col-sm-offset-2 col-sm-6"><div class="blog-slider"><div class="blog-slider__wrp swiper-wrapper"><div class="blog-slider__item swiper-slide"><div class="blog-slider__img ' + color + '"><img src="' + img + '"></div>';
                        cssdata += '<div class="row left-m-t"><div class="blog-slider"><div class="blog-slider__wrp swiper-wrapper"><div class="blog-slider__item swiper-slide"><div class="blog-slider__img ' + color + '"><img src="' + img + '"></div>';
                        cssdata += '<div class="blog-slider__content"><span class="blog-slider__code">' + machineName + '</span><div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Machine on Time:</span> <span class="col-md-4 col-sm-4" style="padding-left: 2px;">15:15:15</span></div>';
                        cssdata += ' <div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Running Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].RunningTime + '</span></div>';
                        cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Cutting Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].CuttingTime + '</span></div>';
                        cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Cycle Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].CycleTime + '</span></div>';
                        cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Exe Prog Name:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].ExeProgramName + '</span></div></div></div>';
                        cssdata += '<div class="blog-slider__item swiper-slide"><div class="blog-slider__img ' + color + '"> <img src="' + img + '"></div><div class="blog-slider__content"><span class="blog-slider__code">' + machineName + '</span>';
                        cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Utilization %:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].Utilization + '</span></div>';
                        cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Idle Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].IdleTime + '</span></div>';
                        cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Minor Losses:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].MinorLossesTime + '</span></div>';
                        cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Total Parts:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].PartsCount + '</span></div></div></div></div></div></div>';
                        cssdata += ' <div class="col-sm-6"><div class="text-center live-cam"></div></div></div>';
                        //cssdata += ' <div class="col-sm-6"><div class="text-center live-cam" id="divLivestream' + i + '"></div></div></div>';
                    }
                    else {

                        c = c + 1;

                        //cssdata1 += '<div class="row left-m-t40" style="border: 1px solid #fff;"><div class="col-sm-offset-2 col-sm-6"><div class="blog-slider"><div class="blog-slider__wrp swiper-wrapper"><div class="blog-slider__item swiper-slide"><div class="blog-slider__img ' + color + '"><img src="' + img + '"></div>';
                        cssdata1 += '<div class="row left-m-t"><div class="blog-slider"><div class="blog-slider__wrp swiper-wrapper"><div class="blog-slider__item swiper-slide"><div class="blog-slider__img ' + color + '"><img src="' + img + '"></div>';
                        cssdata1 += '<div class="blog-slider__content"><span class="blog-slider__code">' + machineName + '</span><div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Machine on Time:</span> <span class="col-md-4 col-sm-4" style="padding-left: 2px;">15:15:15</span></div>';
                        cssdata1 += ' <div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Running Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].RunningTime + '</span></div>';
                        cssdata1 += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Cutting Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].CuttingTime + '</span></div>';
                        cssdata1 += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Cycle Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].CycleTime + '</span></div>';
                        cssdata1 += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Exe Prog Name:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].ExeProgramName + '</span></div></div></div>';
                        cssdata1 += '<div class="blog-slider__item swiper-slide"><div class="blog-slider__img ' + color + '"> <img src="' + img + '"></div><div class="blog-slider__content"><span class="blog-slider__code">' + machineName + '</span>';
                        cssdata1 += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Utilization %:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].Utilization + '</span></div>';
                        cssdata1 += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Idle Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].IdleTime + '</span></div>';
                        cssdata1 += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Minor Losses:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].MinorLossesTime + '</span></div>';
                        cssdata1 += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Total Parts:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].PartsCount + '</span></div></div></div></div></div></div>';
                        cssdata1 += ' <div class="col-sm-6"><div class="text-center live-cam"></div></div>';
                        // cssdata1 += ' <div class="col-sm-6"><div class="text-center live-cam" id="divLivestream'+i+'"></div></div></div>';
                    }
                }


                $(cssdata).appendTo('.MACDET');
                //$(cssdata1).appendTo('.MACDET1');
                $("#logo").css("display", "block");
                //data1 = msg;
                //swiping();
                chart1.dataProvider = MachineUTFDet;
                chart1.validateData();
                chart.dataProvider = SpindleLoadDet;
                chart.validateData();

                //$(livecam).appendTo("#divLivestream0");
                //setTimeout(machines, 60 * 1000);
            }
        });
    }

    
    function GetChart2() {
        $.get('/MConnextDashboard/GetSpindleLoad', {}, function (msg) {
            
            data2 = msg;
            if (msg !== '') {
                chart2.dataProvider = JSON.parse(msg);
                chart2.validateData();
            }
        });
    }
});