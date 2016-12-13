var app = new App(baseUrl, 1);

var siteMode = {
    GQ: 1,
    SQ: 2
}

var _MODE;

function getSolutionUrl(solutionName) {
    return baseUrl + '/solutions/' + solutionName;
}

function getProjectUrl(solutionName, projectName) {
    return getSolutionUrl(solutionName) + '/projects/' + projectName;
}

function getFileUrl(solutionName, projectName, fileName) {
    return getProjectUrl(solutionName, projectName) + '/files/' + fileName;
}

function getSolutionBuildUrl(solutionName) {
    return getSolutionUrl(solutionName) + '/build';
}

function getProjectRunUrl(solutionName, projectName) {
    return getProjectUrl(solutionName, projectName) + '/run';
}

function getSolutionLogUrl(solutionName) {
    return getSolutionUrl(solutionName) + '/logs';
}

function getInstrumentsUrl() {
    return baseUrl + '/instruments/';
}

var _output;
var _buildLog;
var _groups;
var _groupsBySelector;
var _tree;
var _editor;

function getJsonInstruments() {
    getJsonText(getInstrumentsUrl(), _output);
}

function getFullJson() {
    getJsonText(getSolutionLogUrl('SMACrossover'), _output); //TODO solname
}

function clearOutput() {
    _output.clear();
}

//-------------------------

var chart = $('#hchartPaneBar').highcharts();

var _currentSelector;
var _currentExtremes;

function addItemsToChartSelector() {
    createChartSelector();

    var selectors = _groups.getSelectors();

    selectors.forEach(function (item, i) {
        $('#cmbChartSelectorItems').append('<li data-value="' + i + '"><a href="#">' + item + '</a></li>');
    });

    if (selectors.length > 0) {
        $('#cmbChartSelector').combobox('selectByIndex', '0');
        $('#cmbChartSelector').combobox('enable');
        _currentSelector = selectors[0];
    };

    $('#cmbChartSelector').on('changed.fu.combobox', function (evt, data) {
        _currentSelector = data.text;

        if (chart !== undefined) {
            _currentExtremes = chart.xAxis[0].getExtremes();
        }

        prepareChartForSelector(_currentSelector);
        processingGroupEventsAsync();
    });
}

function processingGroupEventsAsync() {
    setTimeout(function () {
        _output.write('w', 'Processing events for ' + _currentSelector);
        $('#cmbChartSelector').combobox('disable');
        chart.showLoading();
    }, 10);

    setTimeout(function () {
        processingGroupEvents();
        $('#cmbChartSelector').combobox('enable');
        chart.hideLoading();

        _output.write('w', 'Processing done');

        if (_currentExtremes === undefined)
            chart.redraw(false);
        else
            chart.xAxis[0].setExtremes(_currentExtremes.min, _currentExtremes.max, true, false);
    }, 100);
}

function prepareChart(destroyIfExists) {
    if ($('#hchartPaneBar').highcharts() !== undefined) {

        if (destroyIfExists !== undefined && destroyIfExists === true) {
            clearCharts(false);
        } else {
            return;
        }
    }

    addItemsToChartSelector();

    var selectors = _groups.getSelectors();
    prepareChartForSelector(selectors[0]);

    setChartSize();
}

function createChartPad(position, group) {
    var pane = {
        id: group.padNumber.toString(),
        labels: {
            align: 'right',
            x: -3
        },
        title: {
            text: group.name
        },
        top: position.top,
        offset: 0,
        endOnTick: false,
        startOnTick: false,
        minPadding: 0.00,
        maxPadding: 0.00,
        height: position.height,
        lineWidth: 2
    };

    return pane;
}

function createSeries(group) {

    var typeSeries = group.getSeriesType();

    var series = {
        name: group.name,
        type: typeSeries,
        yAxis: group.padNumber.toString(),
        data: [],
        animation: false,
        tooltip: {
            valueDecimals: 2
        }
    };


    if (typeSeries === 'candlestick') {
        series.id = 'bars';
    }

    if (typeSeries === 'line') {
        series.color = 'black';
        series.lineWidth = 1;
    }

    if (typeSeries === 'flags') {
        series.onSeries = 'bars';
    };

    if (group.color !== undefined) {
        series.color = group.color;
    }

    //if (group.name === 'Fills') {
    //    series = {
    //        name: group.name,
    //        color: 'red',
    //        type: 'line',
    //        yAxis: group.padNumber.toString(),
    //        data: [],
    //        animation: false,
    //        lineWidth: 0,

    //        marker: {
    //            enabled: false,
    //            radius: 4
    //            }
    //        }
    //}

    return series;
}

function getChartPadPosition(padNumber, totalPads) {
    var firstPadHeight;

    if (totalPads === 1)
        firstPadHeight = 100;
    else if (totalPads === 2)
        firstPadHeight = 70;
    else
        firstPadHeight = 50;

    if (padNumber === 0)
        return {
            top: '0%',
            height: firstPadHeight + '%'
        }
    else {
        var height = (100 - firstPadHeight) / (totalPads - 1);
        var top = firstPadHeight + (height * (padNumber - 1));

        if (padNumber === 2) { //фикс положения при диначеской верстке не будет работать, может быть абослютные значения
            top = top + padNumber - 1;
            height = height + padNumber - 3;
        }

        return {
            top: top + '%',
            height: height + '%'
        };
    }
}

function prepareChartForSelector(selector) {
    _groupsBySelector = Enumerable.From(_groups.items).Where(function (x) { return x.selectorKeyValue === selector }).ToArray();
    var pads = Enumerable.From(_groupsBySelector).Select('$.padNumber').Distinct();
    var totalPads = pads.Count();

    if (pads.Count === 0) {
        return;
    }

    var axis = [];
    var series = [];

    pads.ForEach(function (item, i) {
        var firstGroupForPad = Enumerable.From(_groupsBySelector).First(function (x) { return x.padNumber === item });
        var padPosition = getChartPadPosition(i, totalPads);

        axis.push(createChartPad(padPosition, firstGroupForPad));
    });

    _groupsBySelector.forEach(function (item, i) {
        series.push(createSeries(item));
        item.seriesId = i;
    });

    createNewChart(axis, series);
    chart = $('#hchartPaneBar').highcharts();
}

function getChart() {
    if (chart === undefined) {
        chart = $('#hchartPaneBar').highcharts();
    }
    return chart;
}

function destroyChart(chartInstance){
    if (chartInstance !== undefined && chartInstance !== null) {
        try {
            chartInstance.destroy();
        } catch (e) {

        }
    }
}

function clearCharts(clearGroups) {
    destroyChart(chart);

    if (clearGroups !== undefined && clearGroups === true) {
        _groups.items = [];
    }

    if (_MODE === siteMode.GQ) {
        destroyChart(chart2);
    }
}

function clearData() {
    clearProperties();

    resetFillsTab();

    if (_MODE === siteMode.GQ) {
        resetTransactionTab();
    }

    setSimulationProgress(0, 0);

    _lblSimulationDate.text('01-01-0001 00:00:00');
}

function createNewChart(axis, series) {
    $('#hchartPaneBar').highcharts('StockChart', {
        title: {
            text: ''
        },

        credits: {
            enabled: false
        },

        chart: {
            animation: false,
            spacing: [0, 5, 5, 0]
        },

        navigator: {
            margin: 5
        },

        rangeSelector: {
            buttons: [
                {
                    type: 'hour',
                    count: 1,
                    text: '1h'
                }, {
                    type: 'day',
                    count: 1,
                    text: '1D'
                }, {
                    type: 'all',
                    count: 1,
                    text: 'All'
                }
            ],
            //selected: 1,
            inputEnabled: false
        },

        plotOptions: {
            series: {
                dataGrouping: {
                    enabled: false
                },
                turboThreshold: 0
            },

            line: {
                dataLabels: {
                    enabled: false
                },
                states: {
                    hover: {
                        lineWidth: 1
                    }
                },
                enableMouseTracking: true
            }
        },

        yAxis: axis,

        series: series,

        xAxis: {
            id: '1',
            gridLineWidth: 1,
            lineWidth: 2
        }
    });

}


//---------------------


var uri = baseUrl + '/solutions/';

var currentItem = {
    itemType: '',
    solutionName: '',
    projectName: '',
    fileName: ''
};

var lastFileItem;
var lastFileContent;
var selectedSolution;

var timerUpdateLog;

function chartsValidateData() {

    try {
        getChart();

        if (chart !== undefined && _groupEvents.length > 0) {
            chart.redraw(false);
        }
    }
    catch (e) {
        _output.write('s', 'Error: chart update ' + e.message);
    }
}

function setSimulationProgress(value, timeout) {
    setTimeout(function () {
        if (value !== 0) {
            _prgSimulation.css('transition-duration', '600ms');
            _prgSimulation.css('min-width', '2em');
            _prgSimulation.width(value + '%');
            _prgSimulation.text(value + '%');
        }
        else {
            _prgSimulation.css('transition-duration', '0ms');
            _prgSimulation.css('min-width', '0em');
            _prgSimulation.width(value + '%');
            _prgSimulation.text('');
        };
    }, timeout);
}

function processingDataItem(data) {
    if (data.$type === 'SmartQuant.Output, SmartQuant') {
        _output.write('s', data.text);
    }

    if (data.$type === 'SmartQuant.OnSimulatorProgress, SmartQuant') {
        setSimulationProgress(data.percent, 10);
    }

    if (data.$type === 'SmartQuant.OnSimulatorStart, SmartQuant') {
        setSimulationProgress(0, 0);
        _output.write('s', 'OnSimulatorStart');
    }

    if (data.$type === 'SmartQuant.OnSimulatorStop, SmartQuant') {
        setSimulationProgress(100, 100);
        _output.write('s', 'OnSimulatorStop');
    }

    if (data.$type === 'SmartQuant.GroupEvent, SmartQuant') {
        if (data.Obj.$type === 'SmartQuant.ParameterList, SmartQuant' || data.Obj.$type === 'SmartQuant.StrategyStatusInfo, SmartQuant') {
            return;
        }

        //prepareChart();
        _lblSimulationDate.text(new Date(data.dateTime).format('dd-mm-yyyy HH:MM:ss'));

        _groupEvents.push(data);
        processingGroupEvent(data);
    }

    if (data.$type === 'SmartQuant.Group, SmartQuant') {
        processingGroup(data);
        prepareChart(true);
        processingGroupEvents();
    }
}

var _groupEvents = [];

function processingGroupEvents() {
    _groupEvents.forEach(function (item) {
        processingGroupEvent(item, false);
    });
}

function updateFillsTabText(text) {
    setTimeout(function () {
        if (text === undefined || text === 0)
            $('#tabFills').text('Fills');
        else
            $('#tabFills').text('Fills (' + text + ')');
    }, 10);
}

function resetFillsTab() {
    _gridFills.data().clear();
    _fillCount = 0;

    updateFillsTab();
}

function resetTransactionTab() {
    if (_gridTransactions !== undefined) {
        _gridTransactions.data().clear();
        _gridTransactions.rows().invalidate().draw();
    }
}

function updateFillsTab() {
    setTimeout(function () {
        _gridFills.rows().invalidate().draw();
        updateFillsTabText(_fillCount);
    }, 10);
}

var _fillCount = 0;

function processingFillEvent(data) {

    var r = _groups.items.filter(function (item) {
        return item.id === data.GroupId;
    });

    if (r.length === 0 || r.length > 1) return false;
    if (r[0].name !== 'Fills') return false;

    var title = data.Obj.side === 0 ? 'Buy' : 'Sell';

    var fill = [
        new Date(data.Obj.dateTime).format('dd-mm-yyyy HH:MM:ss'),
        r[0].selectorKeyValue,
        data.Obj.orderId,
        title,
        data.Obj.qty,
        data.Obj.price.toFixed(2),
        data.Obj.commission.toFixed(2),
        data.Obj.text
    ];

    _gridFills.row.add(fill);
    _fillCount++;

    return true;
}

function processingGroupEvent(data, handleFills) {
    if (handleFills !== false)
        processingFillEvent(data);

    var r = _groupsBySelector.filter(function (item) {
        return item.id === data.GroupId;
    });

    if (r.length === 0) return;
    if (r.length > 1) return;

    var group = r[0];
    var series = chart.series[group.seriesId];

    if (group.name === 'Bars') {
        series.addPoint(
            [new Date(data.Obj.dateTime).getTime(), data.Obj.open, data.Obj.high, data.Obj.low, data.Obj.close],
            false, false, false);
    } else if (group.name === 'Fills') {
        var title = data.Obj.side === 0 ? 'Buy' : 'Sell';

        series.addPoint(
        {
            x: new Date(data.Obj.dateTime).getTime(),
            text: title + ' ' + data.Obj.qty + ' \u0040 ' + data.Obj.price + ' ' + data.Obj.text,
            title: title
        }, false, false, false);
    } else {
        series.addPoint(
            [new Date(data.Obj.dateTime).getTime(), data.Obj.value],
            false, false, false);
    }
}

function processingGroup(data) {

    if (data.Fields === undefined) return;
    if (data.Fields.Pad === undefined || data.Fields.SelectorKey === undefined) return;

    var item = new Group(data.Id, data.Name, data.Fields.SelectorKey.value);
    item.padNumber = data.Fields.Pad.value;

    if (data.Fields.Color !== undefined) {
        item.color = 'rgb(' + data.Fields.Color.value + ')';
    }

    _groups.items.push(item);
}

function setRunProjectMenu(changeMenuState) {
    var menuList = $('#run-project-menu');

    if (changeMenuState === true) disableSaveAction();
    menuList.empty();

    $.ajax({
        url: getSolutionUrl(_tree.selectedSolution) + '/projects/',
        success: function (data) {
            $.each(data, function (key, val) {
                menuList.append('<option>' + val.Name + '</option>');
            });
        },
        complete: function () {
            if (changeMenuState === true) enableSaveAction();
        }
    });


}

function showSolution(solutionName, changeMenuState) {
    if (_tree.selectedSolution === solutionName) return;

    if (_MODE === siteMode.SQ) {
        document.title = 'SmartQuant - ' + solutionName;
    } else {
        _lifeInput.clear();
        document.title = 'GQ - ' + solutionName;
    }

    _tree.selectedSolution = solutionName;

    clearEditor();
    clearProperties();


    changeMenuState === undefined ? setRunProjectMenu(true) : setRunProjectMenu(changeMenuState);

    _tree.recreate();

    if (_MODE === siteMode.GQ) {
        _lifeInput.loadLifeObjects();
    }

    //copy-paste -- refactoring
    $("#treeSolution").on('selected.fu.tree', function (event, data) {
        var treeItem = data.selected[0];

        currentItem.itemType = treeItem.itemType;
        currentItem.solutionName = treeItem.solutionName;
        currentItem.projectName = treeItem.projectName;
        currentItem.fileName = '';

        if (treeItem.itemType === 'solution') currentItem.solutionName = treeItem.text;
        if (treeItem.itemType === 'project') currentItem.projectName = treeItem.text;
        if (treeItem.itemType === 'file') currentItem.fileName = treeItem.text;

        loadFile();
    });

}

function clearEditor() {
    lastFileItem = undefined;
    _editor.setValue('');
    _editor.setOption('readOnly', 'nocursor');
    $("#tabEditor").text('Editor');

}

function copyToClipboard() {
    var $temp = $("<input>");
    $("body").append($temp);
    $temp.val($("#txtOutput").val()).select();
    document.execCommand("copy");
    $temp.remove();
}

function clearLogs() {
    $('#txtBuildLog').val('');
    $('#txtOutput').val('');
    $('#txtLogOutput').val('');
    $('#txtProcessOutput').val('');
    $("#txtJson").val('');
}

function buildSolutionAsync() {
    var solutionName = _tree.selectedSolution;

    setTimeout(function () {
        if (lastFileItem != undefined && lastFileItem.solutionName === solutionName) {
            saveFile(false);
        }

        buildSolution(solutionName, getSolutionBuildUrl(solutionName), _output, _buildLog);
    }, 10);
}

function disableSaveAction() {
    $('#save-file').addClass('disabled');
    disableRunAction();
}

function enableSaveAction() {
    $('#save-file').removeClass('disabled');
    enableRunAction();
}

function disableRunAction() {

    $('#run-project').addClass('disabled');
    $('#run-project-menu').prop('disabled', true);
    $('#run-project-menu').selectpicker('refresh');
    $('#build-solution').addClass('disabled');
    $('#diagnostic').addClass('disabled');
}

function enableRunAction() {
    $('#run-project').removeClass('disabled');
    $('#run-project-menu').prop('disabled', false);
    $('#run-project-menu').selectpicker('refresh');
    $('#build-solution').removeClass('disabled');
    $('#diagnostic').removeClass('disabled');
    setStatus('Ready');
}

function setStatus(text) {
    setTimeout(function () {
        _lblStatus.text(text);

        if (_MODE === siteMode.GQ) {
            if (text !== 'Ready') {
                $('#status-bar').css('background-color', '#def0de');
            } else {
                $('#status-bar').css('background-color', '#f5f5f5');
            }
        }
    });
}

function runProjectAsync(projectName) {
    targetUrl = undefined;
    disableRunAction();
    setStatus('Running ' + _tree.selectedSolution + '\\' + projectName + '...');

    clearLogs();
    clearCharts(true);
    clearData();

    if (projectName === undefined || projectName === '')
        projectName = $('#run-project-menu').val();

    setTimeout(function () {
        if (lastFileItem != undefined && lastFileItem.solutionName === _tree.selectedSolution) {
            saveFile(false);
        }

        var isSuccessBuild = buildSolution(_tree.selectedSolution, getSolutionBuildUrl(_tree.selectedSolution), _output, _buildLog, false);
        if (isSuccessBuild) {
            clearLastResultOnServer();
            runProject(projectName);
        }
    }, 10);
}

function startUpdateTimer() {
    writeToOutput('w', "Timer start");

    timerUpdateLog = setInterval(function () {

        if (isBusy === false) {
            checkProcessStatus();
            loadData();
        }

    }, 1000);
}

var processIsRunning = true;

function runProject(projectName, isEmulate ) {
    clearLogs();
    clearCharts(true);
    clearData();

    _groupEvents = [];

    createChartSelector();
    $('#cmbChartSelector').combobox('disable');

    if (isEmulate !== undefined) {
        _output.write('w', 'Emulate Run...');
        startUpdateTimer();
    }

    if (projectName === '') {
        setStatus('Loading log...');
        //step = 10000;
    } else {
        writeToOutput('s', 'Run project ' + _tree.selectedSolution + '\\' + projectName + '...');
        setStatus('Running ' + _tree.selectedSolution + '\\' + projectName + '...');
        //step = 5000;
    }

    disableRunAction();

    start = 0;
    stop = step;
    anyReceived = false;
    timerRun = true;
    isBusy = false;
    stopUpdate = false;
    zeroResponeCounter = 0;
    processIsRunning = true;

    if (_MODE === siteMode.GQ) {
        _lifeInput.saveInput();
    }

    if (isEmulate === undefined) {

        var url = getProjectRunUrl(_tree.selectedSolution, projectName);

        if (_MODE === siteMode.GQ) {
            url += '?waitForExit=true';
        }

        $.ajax({
            url: url,
            async: true,
            success: function (data) {

                if (_MODE === siteMode.SQ) {
                    _output.write('s', 'Process (project) status: ' + data);
                    startUpdateTimer();
                }
                else {
                    _output.write('s', 'Process output:');
                    _output.write('s', data.Output);
                    _output.write('s', data.ErrorOutput);
                    _output.write('s', 'Run completed');

                    getJson();
                }
            },
            error: function (jqxhr, status, errorMsg) {

                if (jqxhr.responseText !== undefined) {
                    _output.write('s', JSON.parse(jqxhr.responseText).Message);
                }

                _output.write('s', 'Run failed');

                enableRunAction();
            }
        });
    }
}

var targetUrl;

$('#logList').on('click', 'a', function (e) {
    _output.write('s', 'Loading log: ' + e.currentTarget.id);
    targetUrl = baseUrl + '/project/' + e.currentTarget.id + '/getLog';
    runProject('', true);
});

function getJson()
{
    var url = baseUrl + '/project/getJson';

    $.ajax({
        url: url,
        async: false,
        success: function(data) {
            _output.write('w', data);

            _gqData = JSON.parse(data);
            processingGQData();
        },
        error: function(data) {
            _output.write('s', 'Error: ' + data);
        },
        complete: function(data) {
            enableRunAction();
        }
        //,
        //xhr: function() {
        //    var xhr = new window.XMLHttpRequest();

        //    xhr.addEventListener('progress', function (evt) {
        //        if (evt.lengthComputable) {
                    
        //            var percentComplete = evt.loaded / evt.total;
        //            alert(percentComplete);
        //            _output.write('s', percentComplete);
        //        }
        //    }, false);

        //    return xhr;
        //}
    });
}

var properties;
var strategies;

function onPropertyChanged(name, newValue, typeName) {
    strategies.onPropertyChanged(name, newValue, typeName);
}

function clearProperties() {
    strategies.strategy = undefined;
    properties.clear();
    $('#userCommand').hide();;
    $('#txtUserCommand').val('');
}

function getStrategyModel(clientId) {
    $.ajax({
        url: baseUrl + '/strategies/' + clientId,
        async: true,
        success: function(data) {
            if (data != null) {

                strategies.strategy = data;
                
                if (properties.isCleared) {
                    if (data.Parameters !== null) {
                        _output.write('s', 'Properties are received');
                        properties.display(data.Parameters);
                        $('#userCommand').show();
                    }
                }

                
            }
        }
    });
}

function checkProcessStatus() {
    setTimeout(function () {
        if (!processIsRunning) return;
        if (stopUpdate) return;

        $.ajax({
            url: baseUrl + '/project/status',
            async: true,
            success: function (data) {
                _output.write('w', 'Check status: ' + data.Status);

                if (data.ClientId > -1) {
                    getStrategyModel(data.ClientId);
                }

                if (data.Status !== 'Running') {
                    processIsRunning = false;
                    _output.write('s', 'Process is not running. Stopping data update.');
                }
            },
            error: function (data) {
                _output.write('w', 'Check status error');
                processIsRunning = false;
            }
        });
    }, 1000);
}

var start = 0;
var step = 5000;
var stop = step;
var anyReceived = false;
var timerRun = false;
var isBusy = false;
var stopUpdate = false;

function processingData() {
    chartsValidateData();
    updateFillsTab();
}

var zeroResponeCounter = 0;

function loadData() {
    
    isBusy = true;

    localStart = start;
    localStop = stop;

    //writeToOutput('w', "New data request, first/last index "  + start + ' / ' + stop);

    var url = getSolutionLogUrl(_tree.selectedSolution);
    if (targetUrl !== undefined) url = targetUrl;

    $.ajax({
        url: url  + '?' + "first=" + start + "&last=" + stop,
        success: function (data) {
            if (data.length !== 0)
                writeToOutput('w', "Data received, records: " + data.length + " (request: " + localStart + ' / ' + localStop + ")");

            $.each(data, function (key, val) {
                if (_DEBUG) {
                    _output.write('w', JSON.stringify(val));
                }

                processingDataItem(val);

                if (val.$type === 'SmartQuant.OnSimulatorStop, SmartQuant') {
                    stopUpdate = true;
                }
            });

            if (data.length === 0 && processIsRunning === false) {
                zeroResponeCounter++;

                if (zeroResponeCounter > 3) {
                    _output.write('w', 'Stop update by zero respone count, proccess is running: ' + processIsRunning);
                    stopUpdate = true;
                }
            }

            if (data.length > 0) {
                zeroResponeCounter = 0;
                anyReceived = true;
                processingData();
            }

            if (data.length === 0 && stopUpdate) {
                writeToOutput('w', "Timer stop");
                clearInterval(timerUpdateLog);
                timerRun = false;
                if (targetUrl === undefined) {
                    app.logs.refresh(true);
                }
                enableRunAction();
                
                processingData();

            } else {
                if (timerRun) {
                    start = start + data.length;
                    stop = start + step;

                    //writeToOutput('w', 'Set new first/last index for request:  ' + start + ' / ' + stop);
                }
            }

            isBusy = false;
        }

    });
}

function processStatusToString(data) {
    if (data === 'NotFound') return 'Process not found';
    if (data === 'Running') return 'Process is running';
    if (data === 'HasExited') return 'Process is already killed';
    if (data === 'Killed') return 'Process is killed';

    return 'Unknown proccess status';
}

function clearLastResultOnServer() {
    var url = _MODE === siteMode.SQ ? baseUrl + '/project/clearLog' : baseUrl + '/project/clearJson';

    $.ajax({
        url: url,
        async: false,
        success: function(data) {
            _output.write('s', data);
        },
        error: function(data) {
            _output.write('s', 'Error: ' + data);
        }
    });
}

function stopProcess() {
    if (strategies.strategy !== undefined) {
        strategies.sendStopScenario();
        return;
    }

    $.ajax({
        url: baseUrl + '/project/stop',
        success: function (data) {
            _output.write('s', processStatusToString(data));
        },
        error: function (data) {
            _output.write('s', 'Error: ' + data);
        }
    });
}

function saveFile(isAsync) {
    var content = _editor.getValue();

    if (_MODE === siteMode.GQ) {
        if ($('#tabInput').parent().hasClass('active')) {
            _lifeInput.saveInput();
            return;
        }
    }

    if (lastFileItem == undefined) return;
    if (lastFileContent === content) return;

    disableSaveAction();

    writeToOutput('w', 'Save file: ' + lastFileItem.solutionName + '\\' + lastFileItem.projectName + '\\' + lastFileItem.fileName);

    $.ajax({
        url: uri + lastFileItem.solutionName + '/projects/' + lastFileItem.projectName + '/files/' + lastFileItem.fileName.replace(".cs", ""),
        type: 'put',
        async: isAsync,
        data: {
            SolutionName: lastFileItem.solutionName,
            ProjectName: lastFileItem.projectName,
            FileName: lastFileItem.fileName,
            Content: content
        },
        success: function (data) {
            writeToOutput('w', 'File saved');
            lastFileContent = content;
        },
        error: function (jqxhr, status, errorMsg) {

            if (jqxhr.responseText !== undefined) {
                writeToOutput('w', JSON.parse(jqxhr.responseText).Message);
            }

            writeToOutput('w', 'Error saving file');
        },
        complete: function () {
            enableSaveAction();
        }
    });
}

function loadFile() {

    if (currentItem.fileName !== '') {

        _editor.setValue('');
        $('#txtEditor').val('');

        lastFileItem = jQuery.extend(true, {}, currentItem);

        $.ajax({
            url: uri + currentItem.solutionName + '/projects/' + currentItem.projectName + '/files/' + currentItem.fileName.replace(".cs", ""),
            success: function (data, result) {
                //setTimeout(function () {
                //    $("#tabEditor").text(currentItem.fileName);
                //    $('#tabs a[href="#editor"]').tab('show');                        
                //}, 10);

                _editor.setValue(data.Content);
                _editor.setOption('readOnly', false);
                $("#tabEditor").text(currentItem.fileName);
                $('#tabs a[href="#editor"]').tab('show');

                lastFileContent = data.Content;
            }
        });
    }
}

function updateMemoryUsage() {
    var p = window.performance.memory;

    if (p != undefined) {

        var r = (p.usedJSHeapSize / 1024 / 1024).toFixed(2) + ' / ' +
        (p.totalJSHeapSize / 1024 / 1024).toFixed(2) + ' / ' +
        (p.jsHeapSizeLimit / 1024 / 1024).toFixed(2);

        $("#lblMemoryUsage").text(r);
    }
}

$("#treeSolution").on('selected.fu.tree', function (event, data) {
    var treeItem = data.selected[0];

    currentItem.itemType = treeItem.itemType;
    currentItem.solutionName = treeItem.solutionName;
    currentItem.projectName = treeItem.projectName;
    currentItem.fileName = '';

    if (treeItem.itemType === 'solution') currentItem.solutionName = treeItem.text;
    if (treeItem.itemType === 'project') currentItem.projectName = treeItem.text;
    if (treeItem.itemType === 'file') currentItem.fileName = treeItem.text;

    //if (currentItem.fileName === '') {
    //    alert(treeItem.attr.id);
    //    $('#treeSolution').tree('toggleFolder', $('#'+treeItem.attr.id));
    //}

    loadFile();
});

var timerMemory = setInterval(function () {
    updateMemoryUsage();
}, 1000);

$(window).unload(function () {
    _output.clear(); //быстрее работает перезагрузка страницы, если много строк в аутпут
});

$(window).resize(function () {
    //setChartSize();
});

function setChartSize() {
    $('#hchartPaneBar').width($('#pages').width() - 25);
}


var cmbChartSelectorHtml;

function createChartSelector() {
    var parentNode = $('#cmbChartSelectorParent');
    if (cmbChartSelectorHtml === undefined) {
        cmbChartSelectorHtml = parentNode.html();
    }
    $('#cmbChartSelector').combobox('destroy');
    parentNode.append(cmbChartSelectorHtml);
    $('#cmbChartSelector').combobox();
}

function initEditor() {
    var el = document.getElementById('txtEditor');
    _editor = CodeMirror.fromTextArea(el, {
        lineNumbers: true,
        matchBrackets: true,
        mode: 'text/x-csharp',
        indentUnit: 4,
        //foldGutter: true,
        //gutters: ["CodeMirror-linenumbers", "CodeMirror-foldgutter"]
    });


    _editor.refresh();
    _editor.display.wrapper.style.fontSize = '13px';
    _editor.display.wrapper.style.fontFamily = 'Monaco, Menlo, "Ubuntu Mono", Consolas, source-code-pro, monospace';
    _editor.setSize('100%', '100%');


    document.getElementById('txtOutput').style.fontSize = '13px';
    document.getElementById('txtBuildLog').style.fontSize = '13px';
}

var _gridFills;

function initGridFills() {
    _gridFills = $('#gridFills').DataTable({
        autoWidth: false,
        info: false,
        scrollY: '425px',
        scrollCollapse: false,
        paging: false,
        dom: '<"fillsTable"t><"fillsSearchbox"f>',
        order: ([0, 'desc']),
        columns: [
            {
                title: 'Date',
                width: '15%'
            },
            {
                title: 'Instrument'
            },
            { title: 'Order Id' },
            { title: 'Side' },
            { title: 'Quantity' },
            {
                title: 'Price',
                width: '10%'

            },
            {
                title: 'Commission',
                width: '10%'
            },
            {
                title: 'Comment',
                width: '20%'
            }
        ]

    });
}

var _prgSimulation;
var _lblSimulationDate;
var _lblStatus;


function getURLParameter(param) {
    var pageUrl = window.location.search.substring(1);
    var urlVariables = pageUrl.split('&');

    for (var i = 0; i < urlVariables.length; i++) {
        var parameterName = urlVariables[i].split('=');
        if (parameterName[0] === param) {
            return parameterName[1];
        }
    }

    return undefined;
}


var _DEBUG = false;

//
//========================================= RESUME UPDATE AFTER LOGIN / REFRESH ============================================
//

function resumeUpdate() {
    $.ajax({
        url: baseUrl + '/project/status',
        async: true,
        success: function (data) {

            if (data.Status === 'Running') {
                var runInfo = data.SolutionName + '\\' + data.ProjectName;

                _output.write('s', 'Process (' + runInfo + ') is running, updating of data...');

                timerRun = true;
                processIsRunning = true;

                showSolution(data.SolutionName, false);

                disableRunAction();
                setStatus('Running ' + runInfo + '...');
                startUpdateTimer();
            } else {
                showSolution('SMACrossover');
            }
        },
        error: function (data) {
            _output.write('w', 'Check status error');
        }
    });
}


//
//========================================= LAYOUT ============================================
//

var _layout;

function createLayout() {
    var eastSize = this.hasOwnProperty('adminPage') ? 360 : 340;

    _layout = $('body')
        .layout({
            closable: true,
            resizable: true,
            slidable: false,
            livePaneResizing: false,
            spacing_open: 10,
            spacing_closed: 10,
            togglerLength_closed: '100%',
            togglerLength_open: '100%',
            resizerTip: 'Close or resize',
            togglerTip_open: 'Close or resize',

           // west__resizerCursor: 'pointer',
           // west__togglerCursor: 'progress',

            north__slidable: false,
            north__resizable: false,
            //north__size: 63 + 41 - 2,
            north__size: 63,
            north__closable: false,

            south__resizable: true,
            south__size: 255,
            south__minSize: 180,
            south__maxSize: 0.7,
            south__onresize: function(pane, $pane, state, options) {
                setSouthElementsSize(state.innerHeight);
            },

            west__minSize: 280,
            west__size: 280,
            west__maxSize: 500,
            west__onresize: function(pane, $pane, state, options) {
                setWestElementsSize(state.innerHeight);
            },

            center__minWidth: 100,
            center__onresize: function(pane, $pane, state, options) {
                setCenterElementsSize(state.innerHeight, state.innerWidth);
            },

            east__size: eastSize,
            east__minSize: 280,
            east__maxSize: 500,
            east__onresize: function (pane, $pane, state, options) {
                setEastElementsSize(state.innerHeight, state.innerWidth);
            },

            stateManagement__enabled: false,
            showDebugMessages: false
        });



    setElementsSize();
}

function setElementsSize() {
    _layout.resizeAll();
}

function setEastElementsSize(height, width) {
    $('#right-pages').height(height - 44);
    $('#properties').height(height - 75);
}

function setCenterElementsSize(height, width) {
    $('#pages').height(height - 44);
    $('#pg-solutions').height(height - 76);

    if (_MODE === siteMode.GQ) {
        $('#pg-input').height(height - 76);
    }
    else {
        $('#pg-home').height(height - 76);
        $('#pg-logs').height(height - 76);
    }

    $('#pg-editor').height(height - 76);
    if (_editor !== undefined)
        _editor.refresh();

    var chartHeight = _MODE === siteMode.SQ ? height - 76 - 28 : height - 60;

    $('#hchartPaneBar').height(chartHeight);
    $('#hchartPaneBar').width(width - 25);

    if (_MODE === siteMode.GQ) {
        $('#hchartPaneBar2').height(chartHeight);
        $('#hchartPaneBar2').width(width - 25);
    }

    if (chart !== undefined) chart.reflow();
    if (_MODE === siteMode.GQ) {
        if (chart2 !== undefined) chart2.reflow();
    }

    $('.dataTables_scrollBody').height(height - 146);
    $('.fillsTable').height(height - 103);

    $('#gridFills').dataTable().api().draw();
    $('#gridStrategies').dataTable().api().draw();


    if (_MODE === siteMode.GQ) {
        $('#gridTransactions').dataTable().api().draw();
    }
}

function setSouthElementsSize(height) {
    $('#log-pages').height(height - 100);
    $('#txtOutput').height(height - 137);
    $('#txtBuildLog').height(height - 137);
}

function setWestElementsSize(height) {
    $('#left-pages').height(height - 44);
    $('.tree').css('max-height', (height - 76 + 32).toString() + 'px');
}

//
//========================================= TREE INSTRUMENTS ==================================
//

function initTreeInstruments() {
    setTimeout(function () {
        var data = getInstrumentsData();
        var treeInstruments = new TreeInstruments($('#treeInstruments'), data);
        treeInstruments.initialize();
    }, 10);
}

function getInstrumentType(typeId) {
    if (typeId === 0) return 'Stock';
    if (typeId === 1) return 'Futures';
    if (typeId === 9) return 'Synthetic';

    return 'Unknown'; //взять типы из sq
}

function getInstrumentsData() {
    var items = [];

    $.ajax({
        url: getInstrumentsUrl(),
        async: false,
        success: function (data, result) {
            data.forEach(function (item) {
                var instrument = {
                    id: item.id,
                    symbol: item.symbol,
                    type: getInstrumentType(item.type)
                }

                items.push(instrument);
            });
        }
    });

    return items;
}

function TreeInstruments(ctrl, data) {
    var control = ctrl;
    var items = data;

    this.initialize = function () {
        control.tree({
            dataSource: this.datasource,
            folderSelect: false
        });
    }

    this.datasource = function (parentData, callback) {
        var treeItems = [];

        if (parentData.text === undefined) {
            var types = Enumerable.From(items).Select('$.type').Distinct().OrderBy().ToArray();

            types.forEach(function (item) {
                treeItems.push({
                    text: item,
                    type: 'folder'
                });
            });
        }
        else {
            var symbols = Enumerable.From(items).Where('$.type==\'' + parentData.text + '\'').Select('$.symbol').OrderBy().ToArray();
            symbols.forEach(function (item) {
                treeItems.push({
                    text: item,
                    type: 'item'
                });

            });
        }

        callback({ data: treeItems });
    }
}

//
//================================== OUTPUT =========================================
//

//todo использовать Output класс
function writeToOutput(type, text, leaveLineDiv) {

    var o = new Output($('#txtOutput'));
    o.write(type, text, leaveLineDiv);
}

//вывод в output

function Output(ctrl) {
    TextArea.apply(this, arguments);

    this.write = function (type, text, notRemoveReturns, async) {
        if (text === undefined) return;

        if (arguments.length === 1) {
            text = type;
            type = 'w';
        }

        if (!_DEBUG && type === 'w') return;

        if (type === 's') type = _MODE === siteMode.SQ ? 'SQServer\t' : 'GQServer\t';
        if (type === 'w') type = _MODE === siteMode.SQ ? 'SQWeb\t' : 'GQWeb\t';

        if (notRemoveReturns === undefined) {
            text = text.replace('\r', '').replace('\n', '');
        }

        var print = function() {
            var r = this.control.val() === '' ? '' : '\n';

            if (_DEBUG)
                this.control.val(this.control.val() + r + new Date().format('HH:MM:ss') + '\t\t' + type + ' \t' + text);
            else
                this.control.val(this.control.val() + r + new Date().format('HH:MM:ss') + '\t ' + text);

            this.control.scrollTop(this.control[0].scrollHeight);
        }.bind(this);

        if (async !== undefined && async === false) {
            print();
        } else {
            setTimeout(function() {
                    print();
                }.bind(this),
                10);
        }
    }
}

function BuildLog(ctrl) {
    TextArea.apply(this, arguments);

    this.write = function (text) {
        setTimeout(function () {
            var r = this.control.val() === '' ? '' : '\n';
            this.control.val(this.control.val() + r + text);
            this.control.scrollTop(this.control[0].scrollHeight);
        }.bind(this), 10);
    }
}

// вывод в editor

function Editor(ctrl) {
    TextArea.apply(this, arguments);

    this.write = function (text) {
        this.control.val(text);
    }
}

//базовый класс

function TextArea(ctrl) {

    this.control = ctrl;

    this.clear = function () {
        this.control.val('');
    }
}

///
/// =================================== TREE ======================================
///

function Tree(baseUrl, ctrl, parent) {
    var control = ctrl;
    var parentNode = parent;
    var url = baseUrl;
    var html = undefined;

    this.selectedSolution = undefined;

    this.initialize = function () {
        html = parentNode.html();
        control.tree({
            dataSource: function () { }, //this.datasource,
            folderSelect: false
        });
    }

    this.datasource = function (parentData, callback) {
        var items = [];

        if (parentData.text === undefined) {
            var obj = {
                text: this.selectedSolution,
                type: 'folder',
                itemType: 'solution',
                solutionName: this.selectedSolution,
                projectName: '',
                attr: {
                    id: 1
                }
            };

            items.push(obj);
            expandFirstElementInTree();
        } else {
            if (parentData.itemType === 'solution') {
                items = loadProjects(parentData.text);
            }

            if (parentData.itemType === 'project') {
                items = loadFiles(parentData.solutionName, parentData.text);
            }
        }

        callback({ data: items });
    }.bind(this);

    function expandFirstElementInTree() {
        setTimeout(function () {
            control.tree('openFolder', $('#1'));
        }, 100);
    }

    function loadProjects(solutionName) {
        return getSolutionsData(url + '/solutions/' + solutionName + '/projects', 'folder', 'project', solutionName, '');
    }

    function loadFiles(solutionName, projectName) {
        return getSolutionsData(url + '/solutions/' + solutionName + '/projects/' + projectName + '/files', 'item', 'file', solutionName, projectName);
    }

    function getSolutionsData(url, type, itemType, solutionName, projectName) {
        var items = [];

        $.ajax({
            url: url,
            async: false,
            success: function (data) {
                $.each(data, function (key, val) {
                    var obj = {
                        text: val.Name,
                        type: type,
                        itemType: itemType,
                        solutionName: solutionName,
                        projectName: projectName,
                        attr: {
                            id: itemType + solutionName + projectName + val.Name
                        }
                    };

                    items.push(obj);
                });
            }
        });

        return items;
    }

    this.recreate = function () {
        control.tree('destroy');
        parentNode.append(html);
        control = $('#treeSolution'); //TODO взять название в конструкторе
        control.tree({
            dataSource: this.datasource,
            folderSelect: false
        });
    }
}

///
/// ============================ GROUP =====================================
///
function Group(id, name, selectorKeyValue) {
    this.id = id;
    this.name = name;
    this.padNumber = 0;
    this.color = undefined;
    this.selectorKeyValue = selectorKeyValue;
    this.seriesId = -1;

    this.getSeriesType = function () {
        if (this.name === 'Bars') return 'candlestick';
        if (this.name === 'Fills') return 'flags';
        return 'line';
    }
}

function Groups() {
    this.items = [];

    //уникальные селекторы
    this.getSelectors = function () {
        var u = {};
        var r = [];

        this.items.forEach(function (item) {
            if (u.hasOwnProperty(item.selectorKeyValue)) {
                return;
            }

            r.push(item.selectorKeyValue);
            u[item.selectorKeyValue] = 1;
        });

        return r;
    }

    this.getGroupsBySelector = function (selector) {
        items.filter(function (item) {

        });

    }
}

///
///=================== DIAGNOSTIC GET JSON ===============================
///

function getJsonText(url, logger) {

    logger.write('w', 'Get JSON...');

    var maxRecords = 1000000;
    var jsonStr = '';

    $.ajax({
        url: url + '?last=' + maxRecords,
        success: function (data) {
            $.each(data, function (key, val) {
                jsonStr = jsonStr + '\n' + JSON.stringify(val) + '\n';
            });

            logger.control.val(jsonStr);
            logger.write('w', 'Get JSON - done, records ' + data.length);
        },
        error: function (jqxhr, status, errorMsg) { //не должно возникать при запущенном файлсервере

            if (jqxhr.responseText !== undefined) {
                logger.write('w', JSON.parse(jqxhr.responseText).Message);
            }

            logger.write('w', 'Get JSON - error');
        }
    });
}

///
/// ====================== BUILD SOLUTION ================================
///

function buildSolution(solutionName, url, logger, buildLogger, isAsync) {

    function writeToLogs(text) {
        logger.write('s', text);
        buildLogger.write(text);
    };

    buildLogger.clear();
    writeToLogs('Build solution ' + solutionName + '...');
    setStatus('Building solution ' + solutionName + '...');

    disableRunAction();

    if (isAsync === undefined) isAsync = true;
    var success = false;

    $.ajax({
        url: url,
        async: isAsync,
        success: function (data) {
            data.Errors.forEach(function (item) {
                var type = item.Type === 0 ? 'Error' : 'Warning';
                var line = 'Line: ' + item.LineNumber + '\t\t';
                var project = '';

                if (item.ProjectFile !== null) {
                    var tokens = item.ProjectFile.split('\\');
                    project = tokens[tokens.length - 1].replace('.csproj', '') + '\t\t';
                    if (project.length < 12) project += '\t';
                } 

                buildLogger.write(type + '\t\t\t' + project + item.File + '\t\t' + line + item.Message);
            });

            if (data.ResultCode === 0) {
                writeToLogs('Build failed');

                $('#log-tabs a[href="#log-pages-build"]').tab('show');
            } else if (data.ResultCode === 1) {
                writeToLogs('Build succeeded');

                success = true;
            } else {
                writeToLogs('Build - unknown result code: ' + data.ResultCode);
            }
        },
        complete: function () {
            enableRunAction();
        }
    });

    return success;
}
