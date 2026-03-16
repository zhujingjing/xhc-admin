

//显示线状图
function showLine(data) {
    // 创建chart
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "类别"; //横轴显示内容
    chart.startDuration = 1;

    // x轴
    var categoryAxis = chart.categoryAxis;
    categoryAxis.dashLength = 1;
    categoryAxis.fillColor = "#ffffff";
    categoryAxis.axisColor = "#888888";
    categoryAxis.labelRotation = 45; //横轴字体倾斜度数

    // x轴值
    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.inside = false;
    valueAxis.axisColor = "#888888";
    chart.addValueAxis(valueAxis);

    // 图
    var graph = new AmCharts.AmGraph();
    graph.valueAxis = valueAxis;
    graph.bullet = "round"; //"none", "round", "square", "triangleUp", "triangleDown", "triangleLeft", "triangleRight", "bubble", "diamond", "xError", "yError" and "custom".
    graph.bulletSize = 8; //大小
    graph.bulletBorderColor = "#FFFFFF";

    graph.lineColor = "#FF0000"; //线条颜色 "00FF00" "0000FF"
    graph.valueField = "数量"; //纵轴显示内容
    graph.balloonText = "[[category]]数量<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
    chart.addGraph(graph);

        if (false) {
            //第二根线
            graph = new AmCharts.AmGraph();
            graph.valueAxis = valueAxis;
            graph.bullet = "round";//
            graph.bulletSize = 8;//
            graph.bulletBorderColor = "#FFFFFF";
            graph.lineColor = "#00FFFF";//
            graph.valueField = "收入";
            graph.balloonText = "[[category]]收入<br><b><span style='font-size:14px;'>[[value]]</span></b>";//
            chart.addGraph(graph);
        }
        if (false) {
            //第3根线
            graph = new AmCharts.AmGraph();
            graph.valueAxis = valueAxis;
            graph.bullet = "round";//
            graph.bulletSize = 8;//
            graph.bulletBorderColor = "#FFFFFF";
            graph.lineColor = "#FF0000";//
            graph.valueField = "支出";
            graph.balloonText = "[[category]]支出<br><b><span style='font-size:14px;'>[[value]]</span></b>";//
            chart.addGraph(graph);
        }

    // 光标  
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.cursorPosition = "mouse";
    chartCursor.graphBulletSize = 1.5;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 0;

    chart.write("divContainer");
};




//显示线状图
function showLine2(data) {
    // 创建chart
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "类别"; //横轴显示内容
    chart.startDuration = 1;

    // x轴
    var categoryAxis = chart.categoryAxis;
    categoryAxis.dashLength = 1;
    categoryAxis.fillColor = "#ffffff";
    categoryAxis.axisColor = "#888888";
    categoryAxis.labelRotation = 45; //横轴字体倾斜度数

    // x轴值
    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.inside = false;
    valueAxis.axisColor = "#888888";
    chart.addValueAxis(valueAxis);

    // 图
    var graph = new AmCharts.AmGraph();
    graph.valueAxis = valueAxis;
    graph.bullet = "round"; //"none", "round", "square", "triangleUp", "triangleDown", "triangleLeft", "triangleRight", "bubble", "diamond", "xError", "yError" and "custom".
    graph.bulletSize = 8; //大小
    graph.bulletBorderColor = "#FFFFFF";

    graph.lineColor = "#FF88C2"; //线条颜色 "00FF00" "0000FF"
    graph.valueField = "数量1"; //纵轴显示内容
    graph.balloonText = "[[category]]访问量<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
    chart.addGraph(graph);

    if (true) {
        //第二根线
        graph = new AmCharts.AmGraph();
        graph.valueAxis = valueAxis;
        graph.bullet = "round";//
        graph.bulletSize = 8;//
        graph.bulletBorderColor = "#FFFFFF";
        graph.lineColor = "#191970";//
        graph.valueField = "数量2"; //纵轴显示内容
        graph.balloonText = "[[category]]聊天次数<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
        chart.addGraph(graph);
    }
    if (true) {
        //第3根线
        graph = new AmCharts.AmGraph();
        graph.valueAxis = valueAxis;
        graph.bullet = "round";//
        graph.bulletSize = 8;//
        graph.bulletBorderColor = "#FFFFFF";
        graph.lineColor = "#00FFFF";//
        graph.valueField = "数量3"; //纵轴显示内容
        graph.balloonText = "[[category]]聊天量<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
        chart.addGraph(graph);
    }

    // 光标  
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.cursorPosition = "mouse";
    chartCursor.graphBulletSize = 1.5;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 0;

    chart.write("divContainer");
};

function showLine3(data) {
    // 创建chart
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "dtm"; //横轴显示内容
    chart.startDuration = 1;

    // x轴
    var categoryAxis = chart.categoryAxis;
    categoryAxis.dashLength = 1;
    categoryAxis.fillColor = "#ffffff";
    categoryAxis.axisColor = "#888888";
    categoryAxis.labelRotation = 45; //横轴字体倾斜度数

    // x轴值
    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.inside = false;
    valueAxis.axisColor = "#888888";
    chart.addValueAxis(valueAxis);

    // 图
    var graph = new AmCharts.AmGraph();
    graph.valueAxis = valueAxis;
    graph.bullet = "round"; //"none", "round", "square", "triangleUp", "triangleDown", "triangleLeft", "triangleRight", "bubble", "diamond", "xError", "yError" and "custom".
    graph.bulletSize = 8; //大小
    graph.bulletBorderColor = "#FFFFFF";

    graph.lineColor = "#FF88C2"; //线条颜色 "00FF00" "0000FF"
    graph.valueField = "total_jy"; //纵轴显示内容
    graph.title = "总人数";
    graph.balloonText = "[[category]]总量<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
    chart.addGraph(graph);

    if (true) {
        //第二根线
        graph = new AmCharts.AmGraph();
        graph.valueAxis = valueAxis;
        graph.bullet = "round";//
        graph.bulletSize = 8;//
        graph.bulletBorderColor = "#FFFFFF";
        graph.lineColor = "#191970";//
        graph.valueField = "l1"; //纵轴显示内容
        graph.title = "体验型人数";
        graph.balloonText = "[[category]]体验型人数<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
        chart.addGraph(graph);
    }
    if (true) {
        //第3根线
        graph = new AmCharts.AmGraph();
        graph.valueAxis = valueAxis;
        graph.bullet = "round";//
        graph.bulletSize = 8;//
        graph.bulletBorderColor = "#FFFFFF";
        graph.lineColor = "#00FFFF";//
        graph.valueField = "l2"; //纵轴显示内容
        graph.title = "长期会员人数";
        graph.balloonText = "[[category]]长期人数<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
        chart.addGraph(graph);
    }
    if (true) {
        //第4根线
        graph = new AmCharts.AmGraph();
        graph.valueAxis = valueAxis;
        graph.bullet = "round";//
        graph.bulletSize = 8;//
        graph.bulletBorderColor = "#FFFFFF";
        graph.lineColor = "#ff0000";//
        graph.valueField = "l3"; //纵轴显示内容
        graph.title = "再次充值人数";
        graph.balloonText = "[[category]]再次充值人数<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
        chart.addGraph(graph);
    }

    // 光标  
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.cursorPosition = "mouse";
    chartCursor.graphBulletSize = 1.5;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 0;

    //图例
    var legend = new AmCharts.AmLegend();
    legend.align = "center";
    chart.addLegend(legend);

    chart.write("divContainer");
};


//显示线状图
function showLine5(data) {
    // 创建chart
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "dtm"; //横轴显示内容
    chart.startDuration = 1;

    // x轴
    var categoryAxis = chart.categoryAxis;
    categoryAxis.dashLength = 1;
    categoryAxis.fillColor = "#ffffff";
    categoryAxis.axisColor = "#888888";
    categoryAxis.labelRotation = 45; //横轴字体倾斜度数

    // x轴值
    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.inside = false;
    valueAxis.axisColor = "#888888";
    chart.addValueAxis(valueAxis);

    // 图
    var graph = new AmCharts.AmGraph();
    graph.valueAxis = valueAxis;
    graph.bullet = "round"; //"none", "round", "square", "triangleUp", "triangleDown", "triangleLeft", "triangleRight", "bubble", "diamond", "xError", "yError" and "custom".
    graph.bulletSize = 8; //大小
    graph.bulletBorderColor = "#FFFFFF";

    graph.lineColor = "#FF0000"; //线条颜色 "00FF00" "0000FF"
    graph.valueField = "rate"; //纵轴显示内容
    graph.balloonText = "[[category]]转化率<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
    chart.addGraph(graph);

    // 光标  
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.cursorPosition = "mouse";
    chartCursor.graphBulletSize = 1.5;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 0;

    chart.write("divContainer");
};

function showLine2l(data) {
    // 创建chart
    var chart = new AmCharts.AmSerialChart();
    chart.color = "#888888";
    chart.dataProvider = data;
    chart.categoryField = "类别"; //横轴显示内容
    chart.startDuration = 1;

    // x轴
    var categoryAxis = chart.categoryAxis;
    categoryAxis.dashLength = 1;
    categoryAxis.fillColor = "#ffffff";
    categoryAxis.axisColor = "#888888";
    categoryAxis.labelRotation = 45; //横轴字体倾斜度数

    // x轴值
    var valueAxis = new AmCharts.ValueAxis();
    valueAxis.inside = false;
    valueAxis.axisColor = "#888888";
    chart.addValueAxis(valueAxis);

    // 图
    var graph = new AmCharts.AmGraph();
    graph.valueAxis = valueAxis;
    graph.bullet = "round"; //"none", "round", "square", "triangleUp", "triangleDown", "triangleLeft", "triangleRight", "bubble", "diamond", "xError", "yError" and "custom".
    graph.bulletSize = 8; //大小
    graph.bulletBorderColor = "#FFFFFF";

    graph.lineColor = "#0000FF"; //线条颜色 "00FF00" "0000FF"
    graph.valueField = "数量"; //纵轴显示内容
    graph.balloonText = "[[category]]总量<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
    chart.addGraph(graph);

    if (true) {
        //第二根线
        graph = new AmCharts.AmGraph();
        graph.valueAxis = valueAxis;
        graph.bullet = "round";//
        graph.bulletSize = 8;//
        graph.bulletBorderColor = "#FFFFFF";
        graph.lineColor = "#ff0000";//
        graph.valueField = "数量2"; //纵轴显示内容
        graph.balloonText = "[[category]]会员再次充值<br><b><span style='font-size:14px;'>[[value]]</span></b>"; //提示信息
        chart.addGraph(graph);
    }
    

    // 光标  
    var chartCursor = new AmCharts.ChartCursor();
    chartCursor.cursorAlpha = 0;
    chartCursor.cursorPosition = "mouse";
    chartCursor.graphBulletSize = 1.5;
    chart.addChartCursor(chartCursor);
    chart.numberFormatter.precision = 0;

    chart.write("divContainer");
};