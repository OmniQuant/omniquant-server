var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var DataUpdater = (function () {
    function DataUpdater(updatableObject) {
        this.async = true;
        this.updatableObject = updatableObject;
    }
    DataUpdater.prototype.update = function () {
        var _this = this;
        $.ajax({
            url: this.url,
            async: this.async,
            success: function (data) {
                _this.updatableObject.onUpdateData(data);
            },
            error: function (a, b, c) {
                var cons = _this.updatableObject.constructor;
                console.log(cons.name);
                //TODO ts output class
            }
        });
    };
    return DataUpdater;
}());
var DataUpdatable = (function () {
    function DataUpdatable() {
        this.dataUpdater = new DataUpdater(this);
    }
    DataUpdatable.prototype.updateData = function () {
        this.dataUpdater.update();
    };
    return DataUpdatable;
}());
var ServerStatus = (function (_super) {
    __extends(ServerStatus, _super);
    function ServerStatus(url) {
        _super.call(this);
        this.lblCpuUsage = $('#lblCpuUsage');
        this.lblMemoryFree = $('#lblMemoryFree');
        this.lblUsers = $('#lblUsers');
        this.lblDisk = $('#lblDiskUsage');
        this.lblRequestsTotal = $('#lblRequestsTotal');
        this.lblRequestsSize = $('#lblRequestsSize');
        this.lblResponsesSize = $('#lblResponsesSize');
        this.dataUpdater.url = url;
    }
    ServerStatus.prototype.display = function (status) {
        debugger;
        this.lblCpuUsage.text(status.cpu);
        this.lblMemoryFree.text(status.memory);
        this.lblUsers.text(status.users);
        this.lblDisk.text(status.disk);
        this.lblRequestsTotal.text(status.requestsTotal);
        this.lblRequestsSize.text(status.requestsInTotal);
        this.lblResponsesSize.text(status.requestsOutTotal);
    };
    ServerStatus.prototype.onUpdateData = function (data) {
        this.display(data);
    };
    return ServerStatus;
}(DataUpdatable));
var ParameterAttributes = (function () {
    function ParameterAttributes() {
        this.category = 'Parameters';
        this.readonly = false;
    }
    return ParameterAttributes;
}());
var Properties = (function () {
    function Properties(strategies) {
        var _this = this;
        this.propGrid = $('#propGrid');
        this.isCleared = true;
        this.propGrid.on('change', 'input', function (f) {
            var name = f.currentTarget.name;
            var newValue = f.currentTarget.value;
            var typeName = '';
            _this.items.forEach(function (item) {
                if (item.Name === name) {
                    typeName = item.TypeName.split(',')[0];
                }
            });
            if (strategies !== undefined) {
                strategies.onPropertyChanged(name, newValue, typeName);
            }
            else if (_this.callback !== undefined) {
                _this.callback(name, newValue, typeName);
            }
        });
        ;
    }
    Properties.prototype.getParameterAttributes = function (attr) {
        var res = new ParameterAttributes();
        if (attr !== undefined) {
            attr.forEach(function (item) {
                if (item.hasOwnProperty('Category')) {
                    res.category = item['Category'];
                }
                if (item.hasOwnProperty('IsReadOnly')) {
                    res.readonly = item['IsReadOnly'];
                }
            });
        }
        return res;
    };
    Properties.prototype.display = function (items) {
        var _this = this;
        this.clear();
        if (this.callback === undefined) {
            $('#userCommand').show();
            if (items !== undefined && items !== null && items.length > 0) {
                this.propGrid.css('padding-bottom', '25px');
            }
            else {
                this.propGrid.css('padding-bottom', '0px');
            }
        }
        this.isCleared = false;
        this.items = items;
        if (items === undefined || items === null)
            return;
        var meta = {};
        var values = {};
        items.forEach(function (item) {
            var attr = _this.getParameterAttributes(item.Attributes);
            meta[item.Name] = {};
            var metaItem = meta[item.Name];
            metaItem['group'] = attr.category;
            metaItem['type'] = 'numeric';
            metaItem['name'] = item.Name;
            var itemType = 'string';
            if (item.TypeName.indexOf('.Int32') > -1 || item.TypeName.indexOf('.Int64') > -1) {
                itemType = 'Int32';
            }
            if (item.TypeName.indexOf('.Boolean') > -1) {
                itemType = 'Boolean';
                metaItem['type'] = 'boolean';
            }
            if (item.TypeName.indexOf('.Double') > 1) {
                itemType = 'Double';
            }
            metaItem['objectType'] = itemType;
            metaItem['placeholder'] = itemType;
            if (attr.readonly) {
                metaItem['type'] = 'label';
            }
            values[item.Name] = item.Value;
        });
        this.propGrid.jqPropertyGrid(values, meta);
    };
    Properties.prototype.clear = function () {
        this.propGrid.empty();
        this.isCleared = true;
    };
    return Properties;
}());
var Strategies = (function (_super) {
    __extends(Strategies, _super);
    function Strategies(url, createProperties) {
        _super.call(this);
        this.singleMode = false;
        if (createProperties) {
            this.properties = new Properties(this);
        }
        this.initGridStrategies();
        this.dataUpdater.url = url;
        this.setEventHandlers();
    }
    Strategies.prototype.onPropertyChanged = function (name, value, type) {
        var row = this.getSelectedRow();
        if (row != undefined) {
            this.onUserCommand("Parameter\t" + row[4] + "\t" + name + "\t" + value + "\t" + type);
        }
    };
    Strategies.prototype.setEventHandlers = function () {
        var _this = this;
        var prevRowId = -1;
        this.gridStrategies.on('click', 'tr', function () {
            var row = _this.getSelectedRow(false);
            if (row !== undefined) {
                _this.displayProperties(row[0]);
                if (row[0] !== prevRowId) {
                    $('#txtUserCommand').val('');
                    prevRowId = row[0];
                }
            }
        });
        $('#btnSendCommand').click(function () {
            _this.onUserCommand($('#txtUserCommand').val());
        });
        $('#btnStopProject').click(function () {
            _this.sendStopScenario();
        });
        $('#btnKillProcess').click(function () {
            _this.sendKillProcess();
        });
    };
    Strategies.prototype.sendStopScenario = function () {
        var row = this.getSelectedRow();
        if (row !== undefined) {
            this.sendCommand(row[0], 10, null, 'Stop Scenario');
        }
    };
    Strategies.prototype.sendKillProcess = function () {
        var row = this.getSelectedRow();
        if (row !== undefined) {
            this.sendCommand(row[0], -1, null, 'Kill Process');
        }
    };
    Strategies.prototype.getSelectedRow = function (showMessageWhenUndefined) {
        if (showMessageWhenUndefined === void 0) { showMessageWhenUndefined = true; }
        if (this.singleMode) {
            if (this.strategy !== undefined) {
                debugger;
                if (this.strategy.Status === 1) {
                    AdminPage.console.write('s', 'Strategy is stopped');
                    return undefined;
                }
                return [
                    this.strategy.Id,
                    this.strategy.WebUser,
                    this.strategy.toLocaleString(),
                    this.strategy.DurationString,
                    this.strategy.Name
                ];
            }
            return undefined;
        }
        var index = this.gridStrategies.row('.selected')[0];
        var row = this.gridStrategies.rows(index).data()[0];
        if (row === undefined && showMessageWhenUndefined) {
            AdminPage.console.write('s', 'Strategy is not selected');
        }
        return row;
    };
    Strategies.prototype.onUserCommand = function (command) {
        debugger;
        var row = this.getSelectedRow();
        if (row === undefined) {
            return;
        }
        if (row[6] === 'Stopped') {
            AdminPage.console.write('s', 'Strategy Id: ' + row[0] + ' is stopped');
            return;
        }
        if (command === undefined || command === '') {
            AdminPage.console.write('s', 'User command is empty');
            return;
        }
        var parameters = [command];
        this.sendCommand(row[0], 28, parameters, 'Send Strategy Command');
    };
    Strategies.prototype.getItemById = function (id) {
        var res = undefined;
        this.items.forEach(function (item) {
            if (item.Id === id)
                res = item;
        });
        return res;
    };
    Strategies.prototype.sendCommand = function (clientId, commandId, parameters, desc) {
        var message = "Strategy Id: " + clientId + " - send command \"" + desc + "\"";
        if (parameters !== null && parameters !== undefined) {
            message += ', parameters - ';
            message += parameters.join(',');
        }
        AdminPage.console.write('s', message);
        var url = this.dataUpdater.url;
        $.ajax({
            url: url,
            method: 'POST',
            async: true,
            data: {
                CommandId: commandId,
                ClientId: clientId,
                Parameters: parameters
            },
            success: function (data) {
                AdminPage.console.write('s', "Command is sent, command Id: " + data);
            },
            error: function (a, b, c) {
                debugger;
                $('#txtOutput').val('Error - send command - ' + commandId + ', clientId - ' + clientId);
            }
        });
    };
    Strategies.prototype.displayProperties = function (id) {
        var item = this.getItemById(id);
        this.properties.display(item.Parameters);
    };
    Strategies.prototype.onUpdateData = function (data) {
        this.display(data.reverse());
    };
    Strategies.prototype.display = function (items) {
        var _this = this;
        this.items = items;
        var needRedraw = false;
        items.forEach(function (item) {
            var ts = new Date(item.Timestamp);
            var status = item.Status === 0 ? '<font color="green">Started</font>' : 'Stopped';
            var strategyInfo = [
                item.Id,
                item.WebUser,
                ts.toLocaleString(),
                item.DurationString,
                item.Name,
                item.Mode,
                status
            ];
            var needAdd = true;
            var _loop_1 = function(i) {
                var row = _this.gridStrategies.rows(i).data()[0];
                //if (row !== undefined && row[6] === 'Stopped') {
                //    continue;
                //}
                if (row !== undefined && row[0] === item.Id) {
                    strategyInfo.forEach(function (item, index) {
                        if (row[index] !== item) {
                            row[index] = item;
                            needRedraw = true;
                        }
                    });
                    needAdd = false;
                    return "break";
                }
            };
            for (var i = 0; i < _this.gridStrategies.data().length; i++) {
                var state_1 = _loop_1(i);
                if (state_1 === "break") break;
            }
            if (needAdd) {
                _this.gridStrategies.row.add(strategyInfo);
                needRedraw = true;
            }
        });
        if (needRedraw) {
            this.gridStrategies.rows().invalidate().draw(false);
            this.updateTabTitle(items.length);
        }
    };
    Strategies.prototype.updateTabTitle = function (count) {
        $('#tab-strategies').text("Strategies " + (count === 0 ? '' : "(" + count + ")"));
    };
    Strategies.prototype.initGridStrategies = function () {
        this.gridStrategies = $('#gridStrategies')
            .DataTable({
            autoWidth: false,
            info: false,
            scrollY: '425px',
            scrollX: true,
            scrollCollapse: false,
            paging: false,
            select: true,
            dom: '<"fillsTable"t><"fillsFooter"<"fillsSearchbox"f><"fillsPage">>',
            order: ([6, 'asc']),
            columns: [
                {
                    title: 'Id',
                    width: '5%'
                },
                {
                    title: 'User',
                    width: '20%'
                },
                {
                    className: 'aligncenter',
                    title: 'Date'
                },
                {
                    className: 'aligncenter',
                    title: 'Duration'
                },
                {
                    className: 'aligncenter',
                    title: 'Name'
                },
                {
                    className: 'aligncenter',
                    title: 'Mode',
                    width: '15%'
                },
                {
                    className: 'aligncenter',
                    title: 'Status',
                    width: '15%'
                }
            ]
        });
        $("div.toolbar").html('<div style="margin-top:8px; float:left"><input type="" class="form-control input" placeholder="User command" id="txtUserCommand" style="width:350px"/><input type= "submit" value= "Send" class="btn btn-default" id= "btnSendCommand" style="margin-left:10px" /></div>');
    };
    return Strategies;
}(DataUpdatable));
var AdminPage = (function () {
    function AdminPage(baseUrl, startUpdate) {
        this.baseUrl = baseUrl;
        if (startUpdate) {
            document.title = 'SmartQuant - Admin Page';
            this.serverStatus = new ServerStatus(baseUrl + '/serverstatus');
            this.strategies = new Strategies(baseUrl + '/strategies', true);
            this.startUpdateServerStatus();
            this.startUpdateStrategies();
        }
    }
    AdminPage.prototype.startUpdateStrategies = function () {
        var _this = this;
        this.strategies.updateData();
        setInterval(function () {
            _this.strategies.updateData();
        }, 2000);
    };
    AdminPage.prototype.startUpdateServerStatus = function () {
        var _this = this;
        this.serverStatus.updateData();
        setInterval(function () {
            _this.serverStatus.updateData();
        }, 2000);
    };
    return AdminPage;
}());
//# sourceMappingURL=admin.js.map