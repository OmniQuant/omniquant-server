/// <reference path="typings/index.d.ts" />
/// <reference path="output.ts"/>
var Logs = (function () {
    function Logs(url) {
        this.items = [];
        this.url = url;
        this._ctrlList = $('#logList');
        this._ctrlPage = $('#pg-logs');
    }
    Logs.prototype.refresh = function () {
        var _this = this;
        $.ajax({
            url: this.url,
            async: true,
            success: function (data) {
                _this.items = data.reverse();
                _this.display();
            },
            error: function (a, b, c) {
                //TODO ts output class
            }
        });
    };
    Logs.prototype.display = function () {
        var _this = this;
        this._ctrlList.empty();
        this._prevDate = new Date(0);
        this.items.forEach(function (item) {
            if (item.solutionName !== '' && item.projectName !== '')
                _this.insertToPage(item);
        });
        this.setPaddingWhenScrollable();
    };
    //@logger
    Logs.prototype.insertToPage = function (item) {
        var html = [];
        var header = item.solutionName + "\\" + item.projectName;
        var date = new Date(item.creationDate);
        var dateString = date.toLocaleString();
        if (date.toLocaleDateString() !== this._prevDate.toLocaleDateString()) {
            html.push("<a href=\"#\" class=\"list-group-item logs-group\">\n                       <p class=\"list-group-item-text\" style=\"\">" + date.toLocaleDateString() + "\n                       </p></a>");
            this._prevDate = date;
        }
        html.push("<a href=\"#\" class=\"list-group-item\" id=\"" + item.name + "\" onclick=\"\">\n            <p class=\"list-group-item-text\" style=\"\">" + header + "<br>\n            <span style=\"font-size:90%;\">" + dateString + "</span></p></a>");
        this._ctrlList.append(html.join('\n'));
    };
    Logs.prototype.setPaddingWhenScrollable = function () {
        var padding = '0px';
        if (this._ctrlPage.get(0).scrollHeight > this._ctrlPage.height()) {
            padding = '10px';
        }
        this._ctrlPage.css('padding-right', padding);
    };
    return Logs;
}());
function logger(target, propertyKey, descriptor) {
    debugger;
    var origMethod = descriptor.value;
    descriptor.value = function logWrapper() {
        debugger;
        alert('call decorator');
        origMethod.apply(this, arguments);
    };
    var method = target.constructor.name + '.' + propertyKey;
    alert(method);
    return descriptor;
}
//# sourceMappingURL=logs.js.map