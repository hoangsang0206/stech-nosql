const loadDistricts = (form, cityCode, districtCode) => {
    $(form).find('.district-select').empty();
    $(form).find('.district-select').append('<option value="" selected>Chọn quận/huyện</option>');
    $(form).find('.ward-select').empty();
    $(form).find('.ward-select').append('<option value="" selected>Chọn xã/phường</option>');

    if (cityCode) {
        $.ajax({
            type: 'GET',
            url: `/api/address/districts/${cityCode}`,
            success: (response) => {
                if (response) {
                    response.map(district => {
                        $(form).find('.district-select').append(`<option value="${district.code}" ${district.code === districtCode ? 'selected' : ''}>${district.name_with_type}</option>`);
                    });
                }
            }
        })
    }
}

const loadWards = (form, districtCode, wardCode) => {
    $(form).find('.ward-select').empty();
    $(form).find('.ward-select').append('<option value="" selected>Chọn xã/phường</option>');

    if (districtCode) {
        $.ajax({
            type: 'GET',
            url: `/api/address/wards/${districtCode}`,
            success: (response) => {
                if (response) {
                    response.map(ward => {
                        $(form).find('.ward-select').append(`<option value="${ward.code}" ${ward.code === wardCode ? 'selected' : ''}>${ward.name_with_type}</option>`);
                    });
                }
            }
        })
    }
}

$('.city-select').on('change', function () {
    const cityCode = $(this).val();
    loadDistricts($(this).closest('form'), cityCode, null);
})

$('.district-select').on('change', function () {
    const districtCode = $(this).val();
    loadWards($(this).closest('form'), districtCode, null);
})