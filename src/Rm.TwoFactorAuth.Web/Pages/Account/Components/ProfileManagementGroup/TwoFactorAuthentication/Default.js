$(function () {
    const $divDisable = $('#divDisableMFA');
    const $divEnable = $('#divEnableMFA');
    const tfl = abp.localization.getResource('TwoFactorAuth');

    function apiUrl(path) {
        return abp.appPath + 'api/rm/two-factor/' + path;
    }

    function showEnabledUi() {
        $divDisable.show();
        $divEnable.hide();
        $('#VerificationCode').val('');
    }

    function showSetupUi() {
        $divDisable.hide();
        $divEnable.show();
        $('#VerificationCode').val('');
        refreshQrImage();
        refreshManualKey();
    }

    function refreshQrImage() {
        $('#mfaQrImage').attr('src', apiUrl('qr') + '?ts=' + Date.now());
    }

    async function getSetup() {
        return abp.ajax({
            url: apiUrl('setup'),
            type: 'GET'
        });
    }

    async function enable(code) {
        return abp.ajax({
            url: apiUrl('enable'),
            type: 'POST',
            data: JSON.stringify({ verificationCode: code })
        });
    }

    async function disable() {
        return abp.ajax({
            url: apiUrl('disable'),
            type: 'POST'
        });
    }

    async function reset() {
        return abp.ajax({
            url: apiUrl('reset'),
            type: 'POST'
        });
    }

    async function refreshUiFromServer() {
        const setup = await getSetup();
        if (setup.isTwoFactorEnabled) {
            showEnabledUi();
        } else {
            showSetupUi();
        }
    }

    function normalizeCode(code) {
        return (code || '').replace(/\s+/g, '').replace(/-/g, '');
    }

    async function enableMfa() {
        const code = normalizeCode($('#VerificationCode').val());
        if (!code) {
            abp.notify.warn(tfl('EnterVerificationCode'));
            return;
        }

        try {
            await enable(code);
            abp.notify.success(tfl('EnableMfaSuccess'));
            await refreshUiFromServer();
        } catch (e) {
            abp.notify.error(e?.message || tfl('VerificationCodeError'));
        }
    }

    async function disableMfa() {
        try {
            await disable();
            abp.notify.success(tfl('DisableMfaSuccess'));
            await refreshUiFromServer();
        } catch (e) {
            abp.notify.error(e?.message || 'Error');
        }
    }

    async function resetMfa() {
        try {
            await reset();
            abp.notify.info(tfl('ResetMfaSuccess'));
            await refreshUiFromServer(); // 會顯示 setup UI 並刷新 QR
        } catch (e) {
            abp.notify.error(e?.message || 'Error');
        }
    }

    async function refreshManualKey() {
        try {
            const result = await abp.ajax({ url: apiUrl('manual-key'), type: 'GET' });
            if (result && result.sharedKey) {
                $('#mfaManualKey').text(result.sharedKey);
                $('#divManualKey').show();
            } else {
                $('#divManualKey').hide();
            }
        } catch {
            $('#divManualKey').hide();
        }
    }


    // Wire events
    $('#btnEnableMFA').on('click', enableMfa);
    $('#btnDisableMFA').on('click', disableMfa);
    $('#btnResetMFA').on('click', resetMfa);
    $('#btnResetMFA2').on('click', resetMfa);
    $('#btnCopyManualKey').on('click', async function () {
        const text = $('#mfaManualKey').text();
        if (!text) return;
        await navigator.clipboard.writeText(text);
        abp.notify.success('Copied');
    });


    if (window.rmTwoFactorInitialEnabled === true) {
        showEnabledUi();
    } else {
        showSetupUi();
    }

    refreshUiFromServer();
});
