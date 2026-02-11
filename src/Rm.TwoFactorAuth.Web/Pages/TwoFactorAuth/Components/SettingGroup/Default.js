
(function ($) {
    $(function () {
    const tfl = abp.localization.getResource('TwoFactorAuth');

    function apiUrl(path) {
        return abp.appPath + 'api/rm/two-factor/' + path;
    }

        async function setting(issuer, enabled) {
            return abp.ajax({
                url: apiUrl('setting'),
                type: 'POST',
                data: JSON.stringify({ 
                    issuer: issuer,
                    EnforcementEnabled: enabled 
                })
        });
    }


    async function saveSetting() {
        const issuer = $('#Issuer').val();
        const enabled = $('#EnforcementEnabled').prop('checked');
        try {
            await setting(issuer, enabled);
            abp.notify.success(tfl('SavedSuccessfully'));
        } catch (e) {
            abp.notify.error(e?.message);
        }
    }

    $('#btnSaveSetting').on('click', saveSetting);
    });
})(jQuery);