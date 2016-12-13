/// <reference path="logs.ts"/>
var App = (function () {
    function App(baseUrl, siteMode) {
        this.runMode = RunMode.Release;
        this.baseUrl = baseUrl;
        this.siteMode = siteMode;
        this.defineRunMode();
        this.init();
    }
    App.prototype.defineRunMode = function () {
        var d = this.getUrlParameter('debug');
        if (d !== undefined && d === 'true') {
            this.runMode = RunMode.Debug;
            $('.diag').css('visibility', 'visible');
        }
    };
    App.prototype.getUrlParameter = function (param) {
        var pageUrl = window.location.search.substring(1);
        var urlVariables = pageUrl.split('&');
        for (var i = 0; i < urlVariables.length; i++) {
            var parameterName = urlVariables[i].split('=');
            if (parameterName[0] === param) {
                return parameterName[1];
            }
        }
        return undefined;
    };
    App.prototype.init = function () {
        this.logs = new Logs(this.baseUrl + '/project/getLogs');
    };
    return App;
}());
var RunMode;
(function (RunMode) {
    RunMode[RunMode["Release"] = 0] = "Release";
    RunMode[RunMode["Debug"] = 1] = "Debug";
})(RunMode || (RunMode = {}));
var SiteMode;
(function (SiteMode) {
    SiteMode[SiteMode["GQ"] = 1] = "GQ";
    SiteMode[SiteMode["SQ"] = 2] = "SQ";
})(SiteMode || (SiteMode = {}));
//# sourceMappingURL=app-main.js.map