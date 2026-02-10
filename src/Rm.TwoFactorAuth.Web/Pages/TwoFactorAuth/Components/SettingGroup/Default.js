
(function ($) {
    $(function () {
    const tfl = abp.localization.getResource('TwoFactorAuth');

    function apiUrl(path) {
        return abp.appPath + 'api/rm/two-factor/' + path;
    }

    async function setting(code) {
        return abp.ajax({
            url: apiUrl('setting'),
            type: 'POST',
            data: JSON.stringify({ Enforcement: code })
        });
    }


    async function saveSetting() {
        const isEnable = $('#EnforcementEnabled').prop('checked');
        try {
            await setting(isEnable);
            abp.notify.success(tfl('SavedSuccessfully'));
        } catch (e) {
            abp.notify.error(e?.message);
        }
    }

    $('#btnSaveSetting').on('click', saveSetting);
    });
})(jQuery);