const calculateOrderShippingFee = (wardCode, districtCode, cityCode) => {
    const shipping_med = $('input[name="DeliveryMethod"]:checked').val() || 'cod';

    $.ajax({
        type: 'GET',
        url: '/api/shipping/fee',
        data: {
            city: cityCode,
            district: districtCode,
            ward: wardCode,
            shipmed: shipping_med
        },
        success: (response) => {
            if (response.status) {
                const shippingFee = parseFloat(response.data.fee);
                const subTotal = parseFloat($('#order-subtotal').data('subtotal'));
                const totalPrice = subTotal + shippingFee;

                $('#order-total').data('total', totalPrice).html(totalPrice.toLocaleString("vi-VN") + 'đ');
                $('#order-shipping-fee').data('shipping-fee', shippingFee).html(shippingFee.toLocaleString('vi-VN') + 'đ');
            }
        }
    })
}

$('input[name="DeliveryMethod"]').on('change', function () {
    const cityCode = $('#recipient-city').val();
    const districtCode = $('#recipient-district').val();
    const wardCode = $('#recipient-ward').val();

    if (cityCode && districtCode && wardCode) {
        calculateOrderShippingFee(wardCode, districtCode, cityCode);
    }
})

$(document).ready(() => {
    const cityCode = $('#hidden-city').val();
    const districtCode = $('#hidden-district').val();
    const wardCode = $('#hidden-ward').val();

    if (cityCode && districtCode && wardCode) {
        $('.checkout-page').find('#recipient-city').val(cityCode)
        loadDistricts('.checkout-page', cityCode, districtCode);
        loadWards('.checkout-page', districtCode, wardCode);

        calculateOrderShippingFee(wardCode, districtCode, cityCode);
    }
})

$('#recipient-ward').on('change', function() {
    const cityCode = $('#recipient-city').val();
    const districtCode = $('#recipient-district').val();
    const wardCode = $(this).val();

    if(cityCode && districtCode && wardCode) {
        calculateOrderShippingFee(wardCode, districtCode, cityCode);
    }
})

$('.form-place-order .form-submit-btn').click(() => {
    const pId = $('#hidden-product').val();

    showWebLoader();

    $.ajax({
        type: 'GET',
        url: `/api/order/check?pId=${pId}`,
        success: (response) => {
            hideWebLoader();

            if (!response.status) {
                setTimeout(() => {
                    showHtmlDialog('warning', null, `<ul class="list-unstyled p-0">${response.data}</ul>`)
                }, 1000)
            } else {
                $('.form-place-order ').submit();
            }
        }
    })
})