$(document).ready(function () {
    $('.MACDETNW').empty();
    
    var MachineUTFDet = '';
    var SpindleLoadDet = '';
    
    getpopup();
    //GetChart2();
    var data1 = '',data2='';
    //setInterval(pageload1, 1000);
    
    

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
    $("#running").on("click", function (event) {
        var clsname = this;
    });
    $("#idle").on("click", function (event) {
        var clsname = this;
    });
    $("#poweroff").on("click", function (event) {
        var clsname = this;
    });
    $("#breakdown").on("click", function (event) {
        var clsname = this;
    });
    $(document).on("change", ".macid", function (event) {
        
    });
   

    function getpopup() {
        $.get("/Dashboard/machineconnectivity", {}, function (msg) {
            $('.MACDETNW').empty();
            //$('.MACDET1').empty();
            if (msg !== '') {
                $("#running").css("display", "none");
                $("#idle").css("display", "none");
                $("#poweroff").css("display", "none");
                $("#breakdown").css("display", "none");
                var data = JSON.parse(msg);
                var cssdata = '';
                var appendingdata = '';
                
                for (var i = 0; i < data.length; i++) {
                   
                    //MachineUTFDet = data[i].MCSM;
                    //SpindleLoadDet = data[i].Spindleloads;
                    //var livecam = '';
                    var machineName = '';   
                    var color = '';
                    if (data[i].Color === 'GREEN') {
                        color = 'dash-green';
                        $("#running").addClass('Machineclk');
                        $("#running").addClass('cls_' + data[i].MachineID + '');
                    }
                    else if (data[i].Color === 'YELLOW') {
                        color = 'dash-amber';
                        $("#idle").addClass('Machineclk');
                        $("#idle").addClass('cls_' + data[i].MachineID + '');
                    }
                    else if (data[i].Color === 'BLUE') {
                        color = 'dash-blue';
                        $("#poweroff").addClass('Machineclk');
                        $("#poweroff").addClass('cls_' + data[i].MachineID + '');
                    }
                    else if (data[i].Color === 'RED') {
                        color = 'dash-red';
                        $("#breakdown").addClass('Machineclk');
                        $("#breakdown").addClass('cls_' + data[i].MachineID + '');
                    }
                    
                    

                        //Ashok cssdata += '<div class="MC"><div class="blog-slider"><div class="blog-slider__wrp swiper-wrapper"><div class="blog-slider__item swiper-slide"><div class="blog-slider__img ' + color + '"><img src="' + img + '"></div>';
                    cssdata += '<div class="modal fade"><div class="MACDETNW"><div class="modal-dialog"><div class="modal-content"><div class="dash-img-border ' + color + '"></div></div></div></div></div>';


                    //cssdata += '<div class="row"><div class="MC"><div class="col-sm-6"><span class="dash-title">' + data[i].MachineName + '</span></div></div></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">Machine Name:</span><span class="col-sm-5 dash-desc" style="padding:0">' + data[i].MachineName + '</span>/div>';
                    


                    //Ashok cssdata += '<div class="blog-slider__content"><span class="blog-slider__code">' + machineName + '</span><div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Machine on Time:</span> <span class="col-md-4 col-sm-4" style="padding-left: 2px;">15:15:15</span></div>';
                    cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">PowerOn Time :</span><span class="col-sm-5 dash-desc" style="padding-left: 2px;">' + data[i].PowerOnTime + '</span></div>';
                         
                        //Ashok cssdata += ' <div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Running Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].RunningTime + '</span></div>';
                        cssdata += ' <div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">Running Time:</span><span class="col-sm-5 dash-desc" style="padding-left: 2px;">' + data[i].RunningTime + '</span></div>';

                        
                        //Ashok cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Cutting Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].CuttingTime + '</span></div>';
                        cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">Cutting Time:</span><span class="col-sm-5 dash-desc" style="padding-left: 2px;">' + data[i].CuttingTime + '</span></div>';

           
                        //Ashok  cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Total Parts:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].PartsCount + '</span></div></div></div></div></div></div>';
                        cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">Total Parts Count:</span><span class="col-sm-5 dash-desc" style="padding-left: 2px;">' + data[i].PartsCount + '</span></div></div></div></div></div></div>';

                        //Ashok cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Idle Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].IdleTime + '</span></div>';
                        cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">Idle Duration:</span><span class="col-sm-5 dash-desc" style="padding-left: 2px;">' + data[i].IdleTime + '</span></div>';

                        //Ashok cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Exe Prog Name:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].ExeProgramName + '</span></div></div></div>';
                        cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">Exe Prog Name:</span><span class="col-sm-5 dash-desc" style="padding-left: 2px;">' + data[i].ExeProgramName + '</span></div></div></div>';

                        //Ashok cssdata += '<div class="blog-slider__title row"><span class="col-md-8 col-sm-8 text-right" style="padding:0">Cycle Time:</span><span class="col-md-4 col-sm-4" style="padding-left: 2px;">' + data[i].CycleTime + '</span></div>';
                        cssdata += '<div class="row dash-dasc-pad1"><span class="col-sm-7 dash-desc text-right" style="padding:0">Cycle Time:</span><span class="col-sm-5 dash-desc" style="padding-left: 2px;">' + data[i].CycleTime + '</span></div>';

                }
                $(cssdata).appendTo('.MACDETNW');
                $("#logo").css("display", "block");
                
            }
        });
    }
    
});