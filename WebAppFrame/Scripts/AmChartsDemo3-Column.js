

//显示柱状图
function showColumn(data) {
    //创建柱状图对象
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.borderColor = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "类别"; //横轴上数据来源
    //chart.rotate = "regular"; //纵轴显示类别
    chart.columnWidth = 0.5; //柱子宽度
    chart.columnSpacing = 0;
    chart.startDuration = 1;

    var categoryAxis = chart.categoryAxis;
    categoryAxis.axisColor = "#888888";
    //横向显示的字体倾斜度
    categoryAxis.labelRotation = 45; // 0 45 90

    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.axisColor = "#888888";
    valueAxis.stackType = "none"; //多个柱子时显示方式  "none", "regular", "100%", "3d"
    chart.addValueAxis(valueAxis);

    var graph = new AmCharts.AmGraph();
    graph.title = "";
    graph.valueField = "数量男"; /////
    graph.type = "column";
    graph.lineAlpha = 0;
    graph.fillAlphas = 1;
    graph.cornerRadiusTop = 0;
    graph.lineColor = "#2994DC"; //2994DC ff0000 00ff00
    graph.balloonText = "<span style='color:#555555;'>[[category]]</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>"
    chart.addGraph(graph);

    //双柱显示效果
    if (1 == 1) {
        graph = new AmCharts.AmGraph();
        graph.title = "";
        graph.valueField = "数量女";
        graph.type = "column";
        graph.lineAlpha = 0;
        graph.fillAlphas = 1;
        graph.cornerRadiusTop = 0;
        graph.cornerRadiusBottom = 0;
        graph.lineColor = "#ff0000";
        graph.balloonText = "<span style='color:#555555;'>[[category]]</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>";
        chart.addGraph(graph);
    }

    //光标
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.zoomable = false;
    chartCursor.categoryBalloonEnabled = false;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 1;

    if (1 == 1) {//是否开启3d效果
        chart.depth3D = 20;
        chart.angle = 30;
    }

    chart.write("divContainer");
}



//显示柱状图
function showColumn3(data) {
    //创建柱状图对象
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.borderColor = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "类别"; //横轴上数据来源
    //chart.rotate = "regular"; //纵轴显示类别
    chart.columnWidth = 0.5; //柱子宽度
    chart.columnSpacing = 0;
    chart.startDuration = 1;

    var categoryAxis = chart.categoryAxis;
    categoryAxis.axisColor = "#888888";
    //横向显示的字体倾斜度
    categoryAxis.labelRotation = 45; // 0 45 90

    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.axisColor = "#888888";
    valueAxis.stackType = "none"; //多个柱子时显示方式  "none", "regular", "100%", "3d"
    chart.addValueAxis(valueAxis);

    var graph = new AmCharts.AmGraph();
    graph.title = "";
    graph.valueField = "数量wx"; /////
    graph.type = "column";
    graph.lineAlpha = 0;
    graph.fillAlphas = 1;
    graph.cornerRadiusTop = 0;
    graph.lineColor = "#00ff00"; //2994DC ff0000 00ff00
    graph.balloonText = "<span style='color:#555555;'>[[category]]微信</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>"
    chart.addGraph(graph);

    //双柱显示效果
    if (1 == 1) {
        graph = new AmCharts.AmGraph();
        graph.title = "";
        graph.valueField = "数量qq";
        graph.type = "column";
        graph.lineAlpha = 0;
        graph.fillAlphas = 1;
        graph.cornerRadiusTop = 0;
        graph.cornerRadiusBottom = 0;
        graph.lineColor = "#2994DC";
        graph.balloonText = "<span style='color:#555555;'>[[category]]QQ</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>";
        chart.addGraph(graph);
    }

    //3柱显示效果
    if (1 == 1) {
        graph = new AmCharts.AmGraph();
        graph.title = "";
        graph.valueField = "数量ot";
        graph.type = "column";
        graph.lineAlpha = 0;
        graph.fillAlphas = 1;
        graph.cornerRadiusTop = 0;
        graph.cornerRadiusBottom = 0;
        graph.lineColor = "#ff0000";
        graph.balloonText = "<span style='color:#555555;'>[[category]]其他</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>";
        chart.addGraph(graph);
    }

    //光标
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.zoomable = false;
    chartCursor.categoryBalloonEnabled = false;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 1;

    if (1 == 1) {//是否开启3d效果
        chart.depth3D = 20;
        chart.angle = 30;
    }

    chart.write("divContainer");
}



//显示柱状图
function showColumn5(data) {
    //创建柱状图对象
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.borderColor = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "dtm"; //横轴上数据来源
    //chart.rotate = "regular"; //纵轴显示类别
    chart.columnWidth = 0.5; //柱子宽度
    chart.columnSpacing = 0;
    chart.startDuration = 1;

    var categoryAxis = chart.categoryAxis;
    categoryAxis.axisColor = "#888888";
    //横向显示的字体倾斜度
    categoryAxis.labelRotation = 45; // 0 45 90

    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.axisColor = "#888888";
    valueAxis.stackType = "none"; //多个柱子时显示方式  "none", "regular", "100%", "3d"
    chart.addValueAxis(valueAxis);

    var graph = new AmCharts.AmGraph();
    //graph.title = "交易总笔数";
    //graph.valueField = "total_jy"; /////交易总笔数
    //graph.type = "column";
    //graph.lineAlpha = 0;
    //graph.fillAlphas = 1;
    //graph.cornerRadiusTop = 0;
    //graph.lineColor = "#00ff00"; //2994DC ff0000 00ff00
    //graph.balloonText = "<span style='color:#555555;'>[[category]]</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>"
    //chart.addGraph(graph);

    

    //3柱显示效果
    if (1 == 1) {
        graph = new AmCharts.AmGraph();
        graph.title = "充1元人数";
        graph.valueField = "l1";
        graph.type = "column";
        graph.lineAlpha = 0;
        graph.fillAlphas = 1;
        graph.cornerRadiusTop = 0;
        graph.cornerRadiusBottom = 0;
        graph.lineColor = "#999";
        graph.balloonText = "<span style='color:#555555;'>[[category]]</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>";
        chart.addGraph(graph);
    }

    //3柱显示效果
    if (1 == 1) {
        graph = new AmCharts.AmGraph();
        graph.title = "充长期会员人数";
        graph.valueField = "l2";
        graph.type = "column";
        graph.lineAlpha = 0;
        graph.fillAlphas = 1;
        graph.cornerRadiusTop = 0;
        graph.cornerRadiusBottom = 0;
        graph.lineColor = "#ff0000";
        graph.balloonText = "<span style='color:#555555;'>[[category]]</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>";
        chart.addGraph(graph);
    }

    //3柱显示效果
    if (1 == 1) {
        graph = new AmCharts.AmGraph();
        graph.title = "转化人数";
        graph.valueField = "l3";
        graph.type = "column";
        graph.lineAlpha = 0;
        graph.fillAlphas = 1;
        graph.cornerRadiusTop = 0;
        graph.cornerRadiusBottom = 0;
        graph.lineColor = "#2994DC";
        graph.balloonText = "<span style='color:#555555;'>[[category]]</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>";
        chart.addGraph(graph);
    }

    ////双柱显示效果
    if (1 == 1) {
        graph = new AmCharts.AmGraph();
        graph.title = "充钱总人数";
        graph.valueField = "total";
        graph.type = "column";
        graph.lineAlpha = 0;
        graph.fillAlphas = 1;
        graph.cornerRadiusTop = 0;
        graph.cornerRadiusBottom = 0;
        graph.lineColor = "#00ff00";
        graph.balloonText = "<span style='color:#555555;'>[[category]]</span><br><span style='font-size:14px'>[[title]]<b>[[value]]</b></span>";
        chart.addGraph(graph);
    }

    //光标
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.zoomable = false;
    chartCursor.categoryBalloonEnabled = false;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 0;

    if (1 == 1) {//是否开启3d效果
        chart.depth3D = 20;
        chart.angle = 30;
    }

    chart.write("divContainer");
}