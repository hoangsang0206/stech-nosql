
const updateCartCount = () => {
    $.ajax({
        type: 'GET',
        url: '/api/cart/count',
        success: (response) => {
            $('.cart-count').html(response.data);
        },
        error: () => {
            $('.cart-count').html(0);
        }
    })
}


const getCartPreview = () => {
    $.ajax({
        type: 'GET',
        url: '/api/cart/all',
        success: (response) => {
            if (response.status) {
                $('.cart-preview').empty();
                response.data.map(item => {
                    let imgSrc = "/images/no-image.jpg";
                    if (item.product.productImages) {
                        imgSrc = item.product.productImages[0].imageSrc;
                    }

                    $('.cart-preview').append(
                        `<div class="cart-preview-item d-flex align-items-center gap-2">
                            <img src="${imgSrc}" />
                            <div class="d-flex flex-column justify-content-between flex-grow-1">
                                <p class="m-0">${item.product.productName}</p>
                                <div class="d-flex align-items-end justify-content-between">
                                    <div class="d-flex gap-3">
                                        <a class="cart-preview-rm" data-product="${item.productId}"" href="javascript:void(0)">Xóa</a>
                                        <span class="m-0">SL: ${item.quantity}</span>
                                    </div>
                                    <span class="cart-preview-price">${item.product.price.toLocaleString("vi-VN")}đ</span>
                                </div>
                            </div>
                        </div>`
                    );
                })
            }
        }
    })
}

$(document).ready(() => {
    updateCartCount();
    getCartPreview();
})



$(document).ready(() => {
    $(document).on('click', '.cart-preview-rm', function () {
        const productId = $(this).data('product');
        if (productId) {
            $.ajax({
                type: 'DELETE',
                url: `/api/cart?id=${productId}`,
                success: (response) => {
                    if (response.status) {
                        getCartPreview();
                        updateCartCount();
                    }
                }
            })
        }
    })
})



$('.add-to-cart-action, .btn-add-to-cart, .click-add-to-card').click(function() {
    const productID = $(this).data('product');

    if (productID) {
        showWebLoader();
        $.ajax({
            type: 'POST',
            url: `/api/cart?id=${productID}`,
            success: (respone) => {
                if (respone.status) {
                    updateCartCount();
                    getCartPreview();
                }
            },
            error: () => { 
                showErrorDialog();
            }
        })

        hideWebLoader()
    }   
})

//---------------------------------
$(document).ready(() => {
    const cartFormInput = $('.cart-form input').toArray();
    cartFormInput.forEach((input) => {
        checkInputValid($(input));
    })

    if ($('#cod-method').is(':checked')) {
        $('.input-ship-info').show();
    }

    $('input[name="shipmethod"]').on('change', () => {
        if ($('#cod-method').is(':checked')) {
            $('.input-ship-info').show();
        }
        else {
            $('.input-ship-info').hide();
        }
    })
})



const updateCartQty = (id, type, qty, input_element) => {
    showWebLoader();
    $.ajax({
        type: 'PUT',
        url: `/api/cart?id=${id}&type=${type}&qty=${qty}`,
        success: (response) => {
            hideWebLoader()
            if (response.status) {
                const shippingFee = parseFloat($('#shipping-fee').data('shipping-fee')) || 0;
                const subTotal = parseFloat(response.data.totalPrice);
                const productTotalPrice = parseFloat(response.data.productTotalPrice);
                const totalPrice = subTotal + shippingFee;

                input_element.val(response.data.quantity);
                $(`.cart-product-total-price[data-product="${id}"]`).html(productTotalPrice.toLocaleString("vi-VN") + 'đ');
                $('#cart-total-price').data('subtotal', subTotal).html(subTotal.toLocaleString("vi-VN") + 'đ');
                $('#total-price').data('total', totalPrice).html(totalPrice.toLocaleString("vi-VN") + 'đ');


                if (response.message) {
                    setTimeout(() => {
                        showHtmlDialog('info', 'Không thể cập nhật', response.message)
                    }, 500);
                }
            } else {
                setTimeout(() => {
                    showHtmlDialog('info', 'Không thể cập nhật', response.message)
                }, 500);
            }
        },
        error: () => {
            hideWebLoader();
            setTimeout(showErrorDialog, 500);
        }
    })
}

$('.update-quantity').click(function() {
    const productID = $(this).data('product');
    const updateType = $(this).data('update');

    if (productID.length > 0 && updateType.length > 0) {
        updateCartQty(productID, updateType, 0, $(this).siblings('input[name="quantity"]'));
    }
})

//--------
$('input[name="quantity"]').focus(function() {
    const currentQty = $(this).val();

    $(this).blur(() => {
        const newQty = $(this).val();
        const productID = $(this).data('product');
        if (newQty != currentQty) {
            updateCartQty(productID, "", newQty, $(this));
        }
    })
})


const calculateShippingFee = (form, wardCode, districtCode, cityCode) => {
    if (cityCode && districtCode && wardCode) {
        const submitBtn = $(form).find('.form-submit-btn');
        const btnHtml = showButtonLoader(submitBtn, '23px', '4px')

        $.ajax({
            type: 'GET',
            url: '/api/shipping/fee',
            data: {
                city: cityCode,
                district: districtCode,
                ward: wardCode,
                shipmed :'cod'
            },
            success: (response) => {
                if (response.status) {
                    const shippingFee = parseFloat(response.data.fee);
                    const subTotal = parseFloat($('#cart-total-price').data('subtotal'));
                    const totalPrice = subTotal + shippingFee;

                    $('#total-price').data('total', totalPrice).html(totalPrice.toLocaleString("vi-VN") + 'đ');
                    $('#shipping-fee, #estimated-shipping-fee').data('shipping-fee', shippingFee).html(shippingFee.toLocaleString('vi-VN') + 'đ');
                }


                hideButtonLoader(submitBtn, btnHtml);
            },
            error: () => {
                hideButtonLoader(submitBtn, btnHtml);
                setTimeout(showErrorDialog, 500);
            }
        })
    }
}

$('.cart-shipping-fee form').submit(function (e) {
    e.preventDefault();

    const cityCode = $(this).find('#city-select').val();
    const districtCode = $(this).find('#district-select').val();
    const wardCode = $(this).find('#ward-select').val();

    calculateShippingFee(this, wardCode, districtCode, cityCode);
})


$('.cart-buy-action.logged-in').click(() => {
    const cityCode = $('.cart-shipping-fee #city-select').val();
    const districtCode = $('.cart-shipping-fee #district-select').val();
    const wardCode = $('.cart-shipping-fee #ward-select').val();

    window.location.href = `/order/checkout?city=${cityCode}&district=${districtCode}&ward=${wardCode}`;
})