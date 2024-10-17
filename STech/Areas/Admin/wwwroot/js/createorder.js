$('.page-search-form form').submit((e) => {
    e.preventDefault();
})


let typingTimeOut;
$('#search-order-products').keyup(function() {
    clearTimeout(typingTimeOut);

    typingTimeOut = setTimeout(() => {
        const search_value = $(this).val();
        const warehouse_id = $('.select-warehouse').find('.page-dropdown-btn').data('selected');

        $.ajax({
            type: 'GET',
            url: `/api/admin/products/search/${search_value}?warehouse_id=${warehouse_id}`,
            success: (response) => {
                $('.search-order-product-results').empty();

                if (!response.status || response.data.length <= 0) {
                    $('.search-order-product-results').removeClass('show');
                } else {
                    $('.search-order-product-results').addClass('show');


                    response.data.products.forEach(product => {
                        const total_qty = product.warehouseProducts.reduce((total, item) => total + item.quantity, 0);
                        if (total_qty <= 0) return;

                        $('.search-order-product-results').append(`
                            <div class="page-search-result-item search-order-product">
                                <input type="radio" id="${product.productId}" name="search-order-product" value="${product.productId}" hidden />
                                <label for="${product.productId}" class="d-flex gap-2">
                                    <img style="width: 2rem; height: 2rem; object-fit: contain" src="${product.productImages[0].imageSrc || '/admin/images/no-image.jpg'}" alt="" />
                                    <span class="text-overflow-1 flex-grow-1">${product.productName} </span>
                                    <span class="fweight-600 ms-2" style="color: var(--primary-color)">${product.price.toLocaleString('vi-VN')}đ</span>
                                </label>
                            </div>
                        `);
                    });
                }
            },
            error: () => {
                $('.search-order-product-results').empty();
                $('.search-order-product-results').removeClass('show');
            }
        })
    }, 300);
})

$('.select-warehouse .page-dropdown-item').click(() => {
    $('.order-product-items').empty();
    calculateTotalPrice();
})

const calculateShippingFee = (ward_code, district_code, city_code) => {
    const ship_med = $('input[name="DeliveryMethod"]:checked').val() || 'cod';

    ward_code = $('#input-recipient-ward-code').val() || ward_code;
    district_code = $('#input-recipient-district-code').val() || district_code;
    city_code = $('#input-recipient-city-code').val() || city_code;

    if (city_code && district_code && ward_code) {
        const submitBtn = $('.btn-create-order');
        const btnHtml = showButtonLoader(submitBtn, '23px', '4px')

        $.ajax({
            type: 'GET',
            url: '/api/shipping/fee',
            data: {
                city: city_code,
                district: district_code,
                ward: ward_code,
                shipmed: ship_med
            },
            success: (response) => {
                if (response.status) {
                    const shippingFee = parseFloat(response.data.fee);
                    $('.shipping-fee').data('shipping', shippingFee).text(`${shippingFee.toLocaleString('vi-VN')}đ`)

                    calculateTotalPrice();
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

const calculateTotalPrice = () => {
    const total_price = $('.order-item-total-price').toArray().reduce((total, item) => total + parseFloat($(item).data('item-total-price') || 0), 0);
    const shipping_fee = parseFloat($('.shipping-fee').data('shipping') || 0);

    $('.subtotal')
        .data('subtotal', total_price)
        .text(`${total_price.toLocaleString('vi-VN')}đ`);
    $('.total-price')
        .data('total', total_price + shipping_fee)
        .text(`${(total_price + shipping_fee).toLocaleString('vi-VN')}đ`);
}

const updateItemQuantity = (product, qty) => {
    const item_element = $(`.order-product-item[data-product="${product.productId}"]`);
    const sale_price = item_element.find('.order-item-sale-price').val();

    const price = () => {
        if (sale_price) {
            return parseFloat(sale_price) * qty;
        }

        return parseFloat(product.price) * qty
    }

    item_element.data('qty', qty);
    item_element.find('.order-item-qty').val(qty);
    item_element.find('.order-item-total-price')
        .data('item-total-price', price())
        .text(`${price().toLocaleString('vi-VN')}đ`);

    calculateTotalPrice();
}

const removeItem = (product_id) => {
    $(`.order-product-item[data-product="${product_id}"]`).remove();
    calculateTotalPrice();
}

$(document).on('change', 'input[name="search-order-product"]', function () {
    if ($(this).prop('checked')) {
        $('.search-order-product-results').empty();
        $('.search-order-product-results').removeClass('show');
        $('#search-order-products').val(null);

        const warehouse_id = $('.select-warehouse').find('.page-dropdown-btn').data('selected');

        const total_products = parseInt($('.order-product-item').length || 0);

        const product_id = $(this).val();
        const existed_product = $(`.order-product-item[data-product="${product_id}"]`);

        showWebLoader();

        $.ajax({
            type: 'GET',
            url: `/api/admin/products/1/${product_id}/${warehouse_id}`,
            success: (response) => {
                if (response.status) {
                    const product = response.data;
                    const total_qty = product.warehouseProducts.reduce((total, item) => total + item.quantity, 0);

                    if (total_qty <= 0) {
                        showDialog('error', 'Không thể thêm sản phẩm', 'Sản phẩm này đã hết hàng');
                        removeItem(product_id);
                        return;
                    }

                    if (!existed_product.length) {
                        $('.order-product-items').append(`
                            <tr class="order-product-item" data-product="${product.productId}" data-qty="1">
                                <td class="py-2">${total_products + 1}</td>
                                <td class="py-2 ps-2">
                                    <div class="d-flex gap-1">
                                        <div>
                                            <img src="${product.productImages[0].imageSrc || '/admin/images/no-image.jpg'}" alt="" style="width: 3rem" />
                                        </div>
                                        <div class="pe-2 overflow-hidden">
                                            <div class="text-overflow-1 fweight-500 w-100" style="font-size: .95rem; font-weight: 500">${product.productName}</div>
                                            <div class="text-price" style="font-size: .95rem;">${product.price.toLocaleString('vi-VN')}đ</div>
                                        </div>
                                    </div>
                                </td>
                                <td class="py-2">
                                    <input type="number" min="1" value="1" class="order-item-input input-quantity order-item-qty" data-product="${product.productId}" />
                                </td>
                                <td class="py-2">
                                    <input style="width: 10rem" type="text" inputmode="decimal" pattern="[0-9,\\.]*" value="${product.price}" class="order-item-input order-item-sale-price" data-product="${product.productId}" />
                                </td>
                                <td class="py-2">
                                    <div class="d-flex justify-content-end align-items-center gap-3">
                                        <div class="text-price order-item-total-price" style="font-size: .95rem;" data-item-total-price="${product.price}">${product.price.toLocaleString('vi-VN')}đ</div>
                                        <a href="javascript:void(0)" class="text-decoration-none text-danger remove-order-item" data-product="${product.productId}">
                                            <i class="fa-regular fa-trash-can"></i>
                                        </a>
                                    </div>
                                </td>
                            </tr>
                        `);
                    } else {
                        let qty = parseInt(existed_product.data('qty') || 1) + 1;
                        if (qty > total_qty) {
                            qty = total_qty;
                            showDialog('error', 'Không thể thêm sản phẩm', `Số lượng vượt quá số lượng tồn kho (còn lại: ${total_qty})`);
                        }

                        updateItemQuantity(product, qty);
                    }

                    calculateTotalPrice();
                    hideWebLoader(300);
                } else {
                    hideWebLoader(0);
                    showDialog('error', 'Không thể thêm sản phẩm', 'Không tìm thấy sản phẩm này trong kho');
                }
            },
            error: () => {
                hideWebLoader(0);
                showErrorDialog();
            }
        })
    }
})

$(document).on('blur', '.order-item-qty', function () {
    const product_id = $(this).data('product');
    let qty = parseInt($(this).val() || 1);
    const warehouse_id = $('.select-warehouse').find('.page-dropdown-btn').data('selected');

    if (qty < 1) {
        qty = 1;
        $(this).val(qty);
    }

    showWebLoader();

    $.ajax({
        type: 'GET',
        url: `/api/admin/products/1/${product_id}/${warehouse_id}`,
        success: (response) => {
            if (response.status) {
                const total_qty = response.data.warehouseProducts.reduce((total, item) => total + item.quantity, 0);
                if (qty > total_qty) {
                    qty = total_qty;
                    showDialog('error', 'Không thể cập nhật số lượng', `Số lượng vượt quá số lượng tồn kho (còn lại: ${total_qty})`);
                }

                updateItemQuantity(response.data, qty);
                hideWebLoader(300);
            } else {
                hideWebLoader(0);
                showDialog('error', 'Không thể cập nhật số lượng', 'Không tìm thấy sản phẩm hoặc sản phẩm này trong kho');
                removeItem(product_id);
            }
        },
        error: () => {
            hideWebLoader(0);
            showErrorDialog();
        }
    })
})

$(document).on('blur', '.order-item-sale-price', function () {
    const product_id = $(this).data('product');
    const warehouse_id = $('.select-warehouse').find('.page-dropdown-btn').data('selected');

    const qty_element = $(`.order-item-qty[data-product="${product_id}"]`);
    let qty = parseInt($(qty_element).val() || 1);
    if (qty < 1) {
        qty = 1;
        $(qty_element).val(qty);
    }

    if (parseInt($(this).val()) < 0) {
        $(this).val(0);
    }

    showWebLoader();

    $.ajax({
        type: 'GET',
        url: `/api/admin/products/1/${product_id}/${warehouse_id}`,
        success: (response) => {
            if (response.status) {
                updateItemQuantity(response.data, qty);
                hideWebLoader(300);
            } else {
                hideWebLoader(0);
                showDialog('error', 'Không thể cập nhật giá', 'Không tìm thấy sản phẩm hoặc sản phẩm này trong kho');
                removeItem(product_id);
            }
        },
        error: () => {
            hideWebLoader(0);
            showErrorDialog();
        }
    })
})

$(document).on('click', '.remove-order-item', function () {
    const product_id = $(this).data('product');
    removeItem(product_id);
})





$('.show-form-add-customer').click(() => {
    showForm('.add-customer');
})

$('#search-customers').keyup(function () {
    clearTimeout(typingTimeOut);

    typingTimeOut = setTimeout(() => {
        const search_value = $(this).val();

        $.ajax({
            type: 'GET',
            url: `/api/admin/customers/search/phone/${search_value}`,
            success: (response) => {
                $('.search-customer-results').empty();

                if (!response.status || response.data.length <= 0) {
                    $('.search-customer-results').removeClass('show');
                } else {
                    $('.search-customer-results').addClass('show');

                    response.data.forEach(customer => {
                        $('.search-customer-results').append(`
                            <div class="page-search-result-item search-order-customer">
                                <input type="radio" id="${customer.customerId}" name="search-customer" value="${customer.customerId}" hidden />
                                <label for="${customer.customerId}" class="d-flex gap-2">
                                    <span class="text-overflow-1 flex-grow-1">${customer.customerName} </span>
                                    <span class="fweight-600 ms-2" style="color: var(--primary-color)">${customer.phone}</span>
                                </label>
                            </div>
                        `);
                    });
                }
            },
            error: () => {
                $('.search-customer-results').empty();
                $('.search-customer-results').removeClass('show');
            }
        })
    }, 300);
})

$(document).on('change', 'input[name="search-customer"]', function () {
    if($(this).prop('checked')) {
        $('.search-customer-results').empty();
        $('.search-customer-results').removeClass('show');
        $('#search-customers').val(null);

        const customer_id = $(this).val();

        showWebLoader();
        $.ajax({
            type: 'GET',
            url: `/api/admin/customers/1/${customer_id}`,
            success: (response) => {
                if (response.status) {
                    const customer = response.data;
                    $('#customer-id').val(customer.customerId)
                    $('#customer-name').text(customer.customerName);
                    $('#customer-phone').text(customer.phone);
                    $('#customer-address').text(`${customer.address}, ${customer.ward}, ${customer.district}, ${customer.province}`)
                        .data('ward', customer.wardCode).data('district', customer.districtCode).data('city', customer.provinceCode);

                    calculateShippingFee(customer.wardCode, customer.districtCode, customer.provinceCode);
                } else {
                    showDialog('error', 'Không tìm thấy khách hàng', response.message);
                }

                hideWebLoader(300);
            },
            error: () => {
                hideWebLoader(0);
                showErrorDialog();
            }
        })   
    }
})

$('.edit-recipient-info').click(() => {
    showForm('.edit-recipient');
})

$('.edit-recipient form').submit(function(e) {
    e.preventDefault();

    const recipient_name = $('#edit-recipient-name').val();
    const recipient_phone = $('#edit-recipient-phone').val();

    const city = $(this).find('.city-select').val();
    const district = $(this).find('.district-select').val();
    const ward = $(this).find('.ward-select').val();
    const address = $(this).find('.specific-address').val();

    if (!recipient_name || !recipient_phone || !city || !district || !ward || !address) {
        return;
    }

    const submit_btn = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(submit_btn, '23px', '4px');

    $.ajax({
        type: 'GET',
        url: `/api/address/address/${ward}/${district}/${city}`,
        success: (response) => {
            if (response.status) {
                const data = response.data;

                $('#recipient-name').text(recipient_name || '-');
                $('#recipient-phone').text(recipient_phone || '-');
                $('#recipient-address').text(`${address}, ${data.ward.name_with_type}, ${data.district.name_with_type}, ${data.city.name_with_type}`);

                $('#input-recipient-name').val(recipient_name);
                $('#input-recipient-phone').val(recipient_phone);
                $('#input-recipient-address').val(address);
                $('#input-recipient-ward-code').val(data.ward.code);
                $('#input-recipient-district-code').val(data.district.code);
                $('#input-recipient-city-code').val(data.city.code);

                calculateShippingFee(null, null, null);

                hideButtonLoader(submit_btn, btn_element);
                clearFormInput($(this));
                closeForm('.edit-recipient');
            }
        },
        error: () => {
            showDialog('error', 'Địa chỉ không hợp lệ', null);
            hideButtonLoader(submit_btn, btn_element);
        }
    })
})

$('.delete-recipient-info').click(() => {
    $('#recipient-name').text('-');
    $('#recipient-phone').text('-');
    $('#recipient-address').text('-');

    $('#input-recipient-name').val(null);
    $('#input-recipient-phone').val(null);
    $('#input-recipient-address').val(null);
    $('#input-recipient-ward-code').val(null);
    $('#input-recipient-district-code').val(null);
    $('#input-recipient-city-code').val(null);

    const ward = $('#customer-address').data('ward');
    const district = $('#customer-address').data('district');
    const city = $('#customer-address').data('city');
    calculateShippingFee(ward, district, city);
})

$('input[name="DeliveryMethod"]').on('change', function () {
    const ward = $('#customer-address').data('ward');
    const district = $('#customer-address').data('district');
    const city = $('#customer-address').data('city');

    calculateShippingFee(ward, district, city);
})


$('.btn-create-order').click(function () {
    const warehouse_id = $('.select-warehouse').find('.page-dropdown-btn').data('selected') || null;
    const customer_id = $('#customer-id').val() || null;

    if (!customer_id) {
        showDialog('warning', 'Chưa có thông tin khách hàng', 'Vui lòng tìm khách hàng qua SĐT hoặc thêm khách hàng mới');
        return;
    }

    const recipient_name = $('#input-recipient-name').val() || null;
    const recipient_phone = $('#input-recipient-phone').val() || null;
    const recipient_address = $('#input-recipient-address').val() || null;
    const ward_code = $('#input-recipient-ward-code').val() || null;
    const district_code = $('#input-recipient-district-code').val() || null;
    const city_code = $('#input-recipient-city-code').val() || null;

    const delivery_method = $('input[name="DeliveryMethod"]:checked').val() || null;

    const note = $('#order-note').val() || null;

    const product_elements = $('.order-product-item').toArray();
    const products = product_elements.map(item => {
        const product_id = $(item).data('product');
        const qty = $(item).find('.order-item-qty').val();
        const sale_price = $(item).find('.order-item-sale-price').val() || null;

        return { ProductId: product_id, Quantity: qty, SalePrice: sale_price };
    })

    if (!products.length) {
        showDialog('warning', 'Chưa chọn sản phẩm');
        return;
    }

    const btn = $(this);
    const btn_element = showButtonLoader(btn, '23px', '4px');

    const data = JSON.stringify({
        Address: recipient_address,
        WardCode: ward_code,
        DistrictCode: district_code,
        CityCode: city_code,
        CustomerId: customer_id,
        RecipientName: recipient_name,
        RecipientPhone: recipient_phone,
        DeliveryMethod: delivery_method,
        WarehouseId: warehouse_id,
        Note: note,
        Products: products
    });

    console.log(data)

    $.ajax({
        type: 'POST',
        url: '/api/admin/orders/create',
        contentType: 'application/json',
        data: data,
        success: (response) => {
            hideButtonLoader(btn, btn_element);
            if (response.status) {
                showDialogWithCallback('success', 'Tạo đơn hàng thành công', 'Đơn hàng đã được tạo thành công', () => {
                    window.location.reload();
                });

            } else {
                showDialog('error', 'Tạo đơn hàng thất bại', response.message);
            }
        },
        error: () => {
            hideButtonLoader(btn, btn_element);
            showErrorDialog();
        }
    })
})