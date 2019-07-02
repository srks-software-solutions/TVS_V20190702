$(document).ready(function () {
    machineutilizationdata();
    OEE();
    //OEEnew();
    //TargetACtual();
    PriorityAlarm();
    LossContributingFactors();

    var ctx = document.getElementById("myChart").getContext('2d');
    var OEEChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ["Availability", "Perfomance", "Quality", "OEE"],
            datasets: [{
                // label: '# of Votes',
                data: [95, 84, 99, 79],
                backgroundColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            legend: {
                display: false
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    var ctx = document.getElementById("myChart2").getContext('2d');
    var LossChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ["Loss Type 1", "Loss Type 2", "Loss Type 3"],
            datasets: [{
                // label: '# of Votes',
                data: [25, 35, 45],
                backgroundColor: [
                    'rgba(150, 59, 75, 1)',
                    'rgba(54, 50, 235, 1)',
                    'rgba(255, 206, 200, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderColor: [
                    'rgba(150,59,75,1)',
                    'rgba(54, 50, 235, 1)',
                    'rgba(255, 206, 200, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            legend: {
                display: false
            },
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });


    //var ctx = document.getElementById("myChart").getContext('2d');
    //var myChart1 = new Chart(ctx, {
    //    type: 'bar',
    //    data: {
    //        labels: ["Availability", "Perfomance", "Quality", "OEE"],
    //        datasets: [{
    //            backgroundColor: [
    //                'rgba(255, 99, 132, 1)',
    //                'rgba(54, 162, 235, 1)',
    //                'rgba(255, 206, 86, 1)',
    //                'rgba(75, 192, 192, 1)',
    //                'rgba(153, 102, 255, 1)',
    //                'rgba(255, 159, 64, 1)'
    //            ],
    //            borderColor: [
    //                'rgba(255,99,132,1)',
    //                'rgba(54, 162, 235, 1)',
    //                'rgba(255, 206, 86, 1)',
    //                'rgba(75, 192, 192, 1)',
    //                'rgba(153, 102, 255, 1)',
    //                'rgba(255, 159, 64, 1)'
    //            ],
    //            borderWidth: 1
    //        }]
    //    },
    //    options: {
    //        legend: {
    //            display: false
    //        },
    //        scales: {
    //            yAxes: [{
    //                ticks: {
    //                    beginAtZero: true
    //                }
    //            }]
    //        }
    //    }
    //});

    
    function machineutilizationdata() {
        $.get("/Dashboard/MachineUtilization", {}, function (msg) {
            $('.MachineUtilization').empty();
              if (msg !== '') {
                
                var data = JSON.parse(msg);
                var mudata = '';
                for (var i = 0; i < data.length; i++) {
                        
               mudata += '<div class="col-sm-1 dash-new-padding">< div class="dash-new-bg" ><div class="dash-new-title">' + data[i].MachineName + '</div><div class="dash-new-body">' + data[i].MachineUtiization + '</div><div class="dash-new-footer">' + data[i].CurrentTime+'</div></div></div>';
                  }
                  $(mudata).appendTo('.MachineUtilization');
            }
        });
    }

    function PriorityAlarm() {
        $.get("/Dashboard/Alarm", {}, function (msg) {
            $('.alarm').empty();
            if (msg !== '') {

                var data = JSON.parse(msg);
                var alarmdata = '';
                var count = 1;
                for (var i = 0; i < data.length; i++) {

                    //alarmdata += '<div class="col-sm-1 dash-new-padding">< div class="dash-new-bg" ><div class="dash-new-title">' + data[i].MachineName + '</div><div class="dash-new-body">' + data[i].MachineUtiization + '</div><div class="dash-new-footer">' + data[i].CurrentTime + '</div></div></div>';
                    alarmdata += '<div class="alarm" ><div class="col-sm-12" style="padding:0;margin-top: 3px;"><div class="dash-newc-bg"><table class="table table-bordered"><tr class="tablebody1"><td>' + count + '<td>' + data[i].MachineID + '</td><td>' + data[i].AlarmNumber + '</td><td>' + data[i].AlarmMessage + '</td><td>' + data[i].AxisNumber + '</td></tr></table></div></div></div>';
                    count++;
                }
                $(alarmdata).appendTo('.alarm');
            }
        });
    }

    function OEE() {
        $.get('/Dashboard/OEE', {}, function (msg) {
            $('.OEE').empty();
            if (msg !== '') {
                var OEEdata = JSON.parse(msg);
                var OEE = '';
                for (var o = 0; o < OEEdata.length; o++)
                    {
                    OEE += '<div class="col-sm-10" style="padding:0 4px 0 4px;">< div class="dash-news-border" ><div class="dash-newsfs"><div class="row"><div class="col-sm-3 text-center">OEE:</div><label class="m-b-0">' + OEEdata[o].OEE + '</div></div></div></div></div>';

                    OEE += '<div class="col-sm-10" style="padding:0 4px 0 4px;">< div class="dash-news-border" ><div class="dash-newsfs"><div class="row"><div class="col-sm-3 text-center">Availability:</div><label class="m-b-0">' + OEEdata[o].Availability + '</div></div></div></div></div>';

                    OEE += '<div class="col-sm-10" style="padding:0 4px 0 4px;">< div class="dash-news-border" ><div class="dash-newsfs"><div class="row"><div class="col-sm-3 text-center">Performance:</div><label class="m-b-0">' + OEEdata[o].Performance + '</div></div></div></div></div>';

                    OEE += '<div class="col-sm-10" style="padding:0 4px 0 4px;">< div class="dash-news-border" ><div class="dash-newsfs"><div class="row"><div class="col-sm-3 text-center">Quality:</div><label class="m-b-0">' + OEEdata[o].quality + '</div></div></div></div></div>';

                    
                }
                $(OEE).appendTo('.OEE');
                OEEChart.dataProvider = JSON.parse(msg);
                OEEChart.validateData();
            }
        });
    }



    //function OEEnew() {
    //    $.get("/Dashboard/OEE", {}, function (msg) {
    //        if (msg !== '') {
    //            var data1 = JSON.parse(msg);

    //            var arr = [];
    //            arr.push(data1.OEE);
    //            arr.push(data1.Availability);
    //            arr.push(data1.Performance);
    //            arr.push(data1.OEE);
    //            myChart1.data.datasets.forEach((dataset) => {
    //                dataset.data.push(data1.OEE);
    //                dataset.data.push(data1.Availability);
    //                dataset.data.push(data1.Quality);
    //                dataset.data.push(data1.Performance);
    //            });
    //            myChart1.update();
    //        }
    //    });
    //}


    function LossContributingFactors() {
        $.get('/Dashboard/ContributingFactors', {}, function (msg) {

            $('.loss').empty();
            if (msg !== '') {
                var lossdata = JSON.parse(msg);
                var loss = '';
                for (var l = 0; l < lossdata.length; l++) {
                    loss += '<div class=" loss">< div class="col-sm-10" style = "padding:0 4px 0 4px;" ><div class="dash-news-border"><div class="dash-newsfs"><div class="row"><div class="col-sm-3 text-center"><label class="m-b-0">' + lossdata[l].LossCodeDescription + '</label></br><label class="m-b-0">' + lossdata[l].LossDurationInHours + '</label></div></div></div></div></div></div>';

                }
                $(loss).appendTo('.loss');
                LossChart.dataProvider = JSON.parse(msg);
                LossChart.validateData();
            }
        });
    }


});