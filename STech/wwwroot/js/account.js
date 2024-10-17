$('.click-show-sidebar').click(() => {
    $('.account-sidebar > ul').toggleClass('show')
})

$('.form-update-user').submit(function (e) {
    e.preventDefault();

    const fullname = $(this).find('#update_fullname').val();
    const dob = $(this).find('#update_dob').val();
    const phonenumber = $(this).find('#update_phonenumber').val();
    const email = $(this).find('#update_email').val();
    const gender = $(this).find('input[name="update_gender"]:checked').val();

    if (fullname && phonenumber && email) {
        const submitBtn = $(e.target).find('.form-submit-btn');
        const btnHtml = showButtonLoader(submitBtn, '23px', '4px')

        $.ajax({
            type: 'POST',
            url: '/api/account/update',
            contentType: 'application/json',
            data: JSON.stringify({
                FullName: fullname,
                Email: email,
                PhoneNumber: phonenumber,
                DOB: dob || null,
                Gender: gender || null
            }),
            success: (response) => {
                if (response.status) {
                    $('.user-fullname').text(fullname);
                    $('.user-email').text(email);
                    closeFormError(this);
                    showDialog('info', 'Cập nhật thành công', null);
                } else {
                    const str = `<span>
                        <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.message + `</span>`;
                    showFormError(this, str);
                    closeFormErrorWithTimeout(this);
                }

                hideButtonLoader(submitBtn, btnHtml);
            },
            error: () => {
                showErrorDialog();
                hideButtonLoader(submitBtn, btnHtml);
            }
        })
    }
})


$('.form-change-password').submit(function (e) {
    e.preventDefault();

    const oldPassword = $(this).find('#old-password').val();
    const newPassword = $(this).find('#new-password').val();
    const confirmPassword = $(this).find('#confirm-new-password').val();

    if (oldPassword && newPassword && confirmPassword) {
        const submitBtn = $(e.target).find('.form-submit-btn');
        const btnHtml = showButtonLoader(submitBtn, '23px', '4px')

        $.ajax({
            type: 'PUT',
            url: '/api/account/password',
            contentType: 'application/json',
            data: JSON.stringify({
                OldPassword: oldPassword,
                NewPassword: newPassword,
                ConfirmPassword: confirmPassword
            }),
            success: (response) => {
                if (response.status) {
                    closeFormError(this);
                    showDialog('info', 'Đổi mật khẩu thành công', null);

                    $(this).find('#old-password').val('');
                    $(this).find('#new-password').val('');
                    $(this).find('#confirm-new-password').val('');
                } else {
                    const str = `<span>
                        <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.message + `</span>`;
                    showFormError(this, str);
                    closeFormErrorWithTimeout(this);
                }

                hideButtonLoader(submitBtn, btnHtml);
            },
            error: () => {
                showErrorDialog();
                hideButtonLoader(submitBtn, btnHtml);
            }
        })
    }
})


let cropper;
const showCropper = (image) => {
    $('#user-image').val(null);
    if (cropper) {
        cropper.destroy();
    }

    const reader = new FileReader();
    reader.onload = function (e) {
        $('#image-to-edit').attr('src', e.target.result);
        cropper = new Cropper($('#image-to-edit')[0], {
            aspectRatio: 1,
            zoomable: false,
            scaleable: false,
        })
    }

    reader.readAsDataURL(image);

    $('.upload-image').removeClass('show');
    $('.edit-image').addClass('show');
}

const hideCropper = () => {
    cropper.destroy();
    $('#image-to-edit').removeAttr('src');
    $('.edit-image').removeClass('show');
    $('.upload-image').addClass('show');
}

const hideFormUploadImage = () => {
    $('.drag-and-click-upload-image').removeClass('hide');
    $('.preview-image-wrapper').removeClass('show');
    $('#preview-image').removeAttr('src');
    $('.upload-image').removeClass('show');
    $('#user-image').val(null);
    $('#user-image-cropped').val(null);
    $('.form-upload-image').find('.form-submit-btn').prop('disabled', true);
}

$('.form-edit-image').submit(function (e) {
    e.preventDefault();
    const canvas = cropper.getCroppedCanvas();
    if (canvas) {
        canvas.toBlob((blob) => {
            const croppedImage = new File([blob], 'user-image.png', { type: 'image/png' });
            const inputFiles = new DataTransfer();
            inputFiles.items.add(croppedImage);
            $('#user-image-cropped')[0].files = inputFiles.files;
            $('.drag-and-click-upload-image').addClass('hide');
            $('.preview-image-wrapper').addClass('show');
            $('#preview-image').attr('src', URL.createObjectURL(croppedImage))
            $('.form-upload-image').find('.form-submit-btn').removeAttr('disabled');
            hideCropper();
        })
    }
})

$('.form-edit-image').on('reset', function () {
    hideCropper();
})


$('.click-change-image').click(() => {
    $('.upload-image').addClass('show');
})

$('.drag-and-click-upload-image').on('dragover dragenter', function (e) {
    $(this).addClass('active');
    e.preventDefault();
})

$('.drag-and-click-upload-image').on('dragleave dragend drop', function () {
    $(this).removeClass('active');
})

$('.drag-and-click-upload-image, .click-choose-image').click(() => {
    $('#user-image').click();
})

$('.drag-and-click-upload-image').on('drop', (e) => {
    e.preventDefault();
    const file = e.originalEvent.dataTransfer.files[0];
    if (file) {
        showCropper(file);
    }
})

$('#user-image').change(function (e) {
    const file = e.target.files[0];
    if (file) {
        showCropper(file);
    }
})

$('.form-upload-image').submit(function (e) {
    e.preventDefault();
    const formData = new FormData();
    const file = $(this).find('#user-image-cropped').prop('files')[0];
    formData.append('file', file);

    if (formData) {
        const submitBtn = $(this).find('.form-submit-btn');
        const btnHtml = showButtonLoader(submitBtn, '23px', '4px');
        $.ajax({
            type: 'POST',
            url: '/api/account/upload',
            data: formData,
            contentType: false,
            processData: false,

            success: (response) => {
                if (response.status) {
                    $('.user-image img').attr('src', response.data);
                    showDialog('info', 'Cập nhật ảnh đại diện thành công', null);
                    closeFormError(this);
                    hideFormUploadImage();
                } else {
                    const str = `<span>
                        <i class="fa-solid fa-circle-exclamation"></i>`
                                + response.message + `</span>`;
                    showFormError(this, str);
                    closeFormErrorWithTimeout(this);
                }

                hideButtonLoader(submitBtn, btnHtml);
            },
            error: () => {
                showErrorDialog();
                hideButtonLoader(submitBtn, btnHtml);
            }
        });
    }
})

$('.form-upload-image').on('reset', function () {
    hideFormUploadImage();
})

const loadUserAddresses = () => {
    showWebLoader();
    $.ajax({
        type: 'GET',
        url: '/api/account/address',
        success: (response) => {
            if (response.status) {
                $('.user-address-list').empty();
                response.data.map(address => {
                    const isDefault = address.isDefault;
                    $('.user-address-list').append(`
                        <div class="row mt-2">
                            <div class="col-8 col-sm-9 d-flex flex-column justify-content-center">
                                <div class="d-flex align-items-center">
                                    <div class="d-flex align-items-center">
                                        ${isDefault ? `<div class="address-default me-2">Mặc định</div>` : ''}
                                        <span class="fw-bold">${address.recipientName}</span>
                                    </div> 
                                    &nbsp; | &nbsp;
                                    <span class="fw-bold text-secondary">${address.recipientPhone}</span>
                                </div>
                                <div class="mt-2">
                                    ${address.address}, ${address.ward}, ${address.district}, ${address.province}
                                </div>
                            </div>
                            <div class="col-4 col-sm-3 d-flex flex-column align-items-end">
                                ${!isDefault ?
                                    `<div class="d-flex gap-1">
                                        <button class="update-address btn btn-info text-white" data-address="${address.udId}">
                                            <i class="fa-regular fa-pen-to-square" style="font-size: 0.9rem"></i>
                                        </button>
                                        <button class="delete-address btn btn-danger" data-address="${address.udId}">
                                            <i class="fa-regular fa-trash-can" style="font-size: 0.9rem"></i>
                                        </button>
                                    </div>
                                    <div class="mt-1">
                                        <button class="set-address-default btn btn-outline-success" style="font-size: 0.9rem" data-address="${address.udId}">
                                            Đặt làm mặc định
                                        </button>
                                    </div>`
                                    :
                                    `<div>
                                        <button class="update-address btn btn-info text-white" data-address="${address.udId}">
                                            <i class="fa-regular fa-pen-to-square" style="font-size: 0.9rem"></i>
                                        </button>
                                    </div>`
                                    }
                            </div>
                        </div >
                        <hr />
                    `);
                })
            }

            hideWebLoader();
        }
    })
}

const loadOrders = (type) => {
    showWebLoader();
    $.ajax({
        type: 'GET',
        url: `/api/orders/userorders?type=${type}`,
        success: (response) => {
            if (response.status) {
                $('.user-order-list').empty();
                $('.user-order-list').append(`
                    <tr class="page-table-header">
                        <th>STT</th>
                        <th>Mã đơn hàng</th>
                        <th>Ngày đặt</th>
                        <th>Số SP</th>
                        <th>Thành tiền</th>
                        <th>Thanh toán</th>
                        <th>Trạng thái</th>
                        <th></th>
                    </tr>
                `);

                let index = 0;

                response.data.map(order => {
                    index++;

                    const order_date = new Date(order.orderDate);
                    const total_items = order.invoiceDetails.reduce((accumulator, item) => {
                        return accumulator + item.quantity;
                    }, 0);
 
                    let payment_status = ``;
                    let order_status = ``;

                    switch (order.paymentStatus) {
                        case 'paid':
                            payment_status = `<span class="page-badge badge-success">Đã thanh toán</span>`;
                            break;

                        case 'unpaid':
                            payment_status = `<span class="page-badge badge-warning">Chờ thanh toán</span>`;
                            break;

                        case 'payment-failed':
                            payment_status = `<span class="page-badge badge-error">Thanh toán thất bại</span>`;
                            break;
                    }


                    if (order.isCancelled === true) {
                        order_status = `<span class="page-badge badge-error">Đã hủy</span>`;
                    }
                    else {
                        order_status = order.isCompleted ? `<span class="page-badge badge-success">Đã giao hàng</span>`
                            : `<span class="page-badge badge-warning">Chờ giao hàng</span>`;
                    }

                    $('.user-order-list').append(`
                        <tr>
                            <td>${index}</td>
                            <td>${order.invoiceId}</td>
                            <td>${order_date.getDate()}/${order_date.getMonth() + 1}/${order_date.getFullYear()}</td>
                            <td>${total_items}</td>
                            <td class="fweight-600">${order.total.toLocaleString('vi-VN')}đ</td>
                            <td>${payment_status}</td>
                            <td>${order_status}</td>
                            <td>
                                <a class="text-nowrap" href="/order/detail/${order.invoiceId}">Chi tiết</a>
                            </td>
                        </tr>
                    `);
                })
            }

            hideWebLoader();
        }
    })
    hideWebLoader();
}

const loadReviews = () => {
    showWebLoader();
    $.ajax({
        type: 'GET',
        url: '/api/reviews/user',
        success: (response) => {
            if (response.status) {
                
            }

            hideWebLoader();
        }
    })
    hideWebLoader();
}

const showContent = () => {
    const idFromUrl = window.location.hash.substring(1);
    if (idFromUrl) {
        $('.account-content').removeClass('current');
        $(`.account-content[data-sidebar="${idFromUrl}"]`).addClass('current');

        $('.account-sidebar-item').removeClass('active');
        $(`.account-sidebar-item[data-sidebar="${idFromUrl}"]`).addClass('active');

        switch (idFromUrl) {
            case 'addresses':
                loadUserAddresses();
                break;
            case 'orders':
                loadOrders(null);
                break;
            case 'reviews':
                loadReviews();
                break;
        }
    }
}

showContent();

$(window).on('hashchange', () => {
    showContent();
})


$('.btn-add-address').click(() => {
    $('.add-address').addClass('show');
})

$('.form-add-address').submit(function (e) {
    e.preventDefault();

    const recipientName = $(this).find('#RecipientName').val();
    const recipientPhone = $(this).find('#RecipientPhone').val();
    const cityCode = $(this).find('#city-select').val();
    const districtCode = $(this).find('#district-select').val();
    const wardCode = $(this).find('#ward-select').val();
    const address = $(this).find('#Address').val();
    const addressType = $(this).find('input[name="address-type"]:checked').val();

    if (recipientName && recipientPhone && cityCode && districtCode && wardCode && address && addressType) {
        $.ajax({
            type: 'POST',
            url: '/api/account/address',
            contentType: 'application/json',
            data: JSON.stringify({
                RecipientName: recipientName,
                RecipientPhone: recipientPhone,
                CityCode: cityCode,
                DistrictCode: districtCode,
                WardCode: wardCode,
                Address: address,
                Type: addressType
            }),
            success: (response) => {
                if (response.status) {
                    $(this).closest('.form-container').removeClass('show');
                    clearFormInput($(this))
                    loadUserAddresses();
                }
            },
            error: () => {
                showErrorDialog();
            }
        })
    }
})

$(document).on('click', '.set-address-default', function () {
    const addressId = $(this).data('address');
    if (addressId) {
        $.ajax({
            type: 'PUT',
            url: `/api/account/address/default/${addressId}`,
            success: (response) => {
                if (response.status) {
                    loadUserAddresses();
                }
            },
            error: () => {
                showErrorDialog();
            }
        })
    }
})

$(document).on('click', '.update-address', function () {
    const addressId = $(this).data('address');
    if (addressId) {
        $.ajax({
            type: 'GET',
            url: `/api/account/address/${addressId}`,
            success: (response) => {
                if (response.status) {
                    const address = response.data;
                    $('.update-address').addClass('show');
                    const form = $('.form-update-address');

                    form.find('#AddressId').val(address.udId);
                    form.find('#UpdateRecipientName').val(address.recipientName).trigger('change');
                    form.find('#UpdateRecipientPhone').val(address.recipientPhone).trigger('change');
                    form.find('#update-city-select').val(address.provinceCode);
                    form.find('#UpdateAddress').val(address.address).trigger('change');
                    form.find(`input[name="address-type"][value="${address.addressType}"]`).prop('checked', true);

                    loadDistricts(form, address.provinceCode, address.districtCode);
                    loadWards(form, address.districtCode, address.wardCode);
                }
            },
            error: () => {
                showErrorDialog();
            }
        })
    }
})

$('.form-update-address').submit(function (e) {
    e.preventDefault();

    const addressId = $(this).find('#AddressId').val();
    const recipientName = $(this).find('#UpdateRecipientName').val();
    const recipientPhone = $(this).find('#UpdateRecipientPhone').val();
    const cityCode = $(this).find('#update-city-select').val();
    const districtCode = $(this).find('#update-district-select').val();
    const wardCode = $(this).find('#update-ward-select').val();
    const address = $(this).find('#UpdateAddress').val();
    const addressType = $(this).find('input[name="address-type"]:checked').val();

    if (addressId && recipientName && recipientPhone && cityCode && districtCode && wardCode && address && addressType) {
        $.ajax({
            type: 'PUT',
            url: '/api/account/address/update',
            contentType: 'application/json',
            data: JSON.stringify({
                Id: addressId,
                RecipientName: recipientName,
                RecipientPhone: recipientPhone,
                CityCode: cityCode,
                DistrictCode: districtCode,
                WardCode: wardCode,
                Address: address,
                Type: addressType
            }),
            success: (response) => {
                if (response.status) {
                    $(this).closest('.form-container').removeClass('show');
                    clearFormInput($(this))
                    loadUserAddresses();
                }
            },
            error: () => {
                showErrorDialog();
            }
        })
    }
})

$(document).on('click', '.delete-address', function () {
    const addressId = $(this).data('address');
    if (addressId) {
        Swal.fire({
            icon: 'question',
            title: 'Xoá địa chỉ này?',
            showConfirmButton: true,
            showCancelButton: true,
            confirmButtonText: 'Xóa',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    type: 'DELETE',
                    url: `/api/account/address/${addressId}`,
                    success: (response) => {
                        if (response.status) {
                            loadUserAddresses();
                        }
                    },
                    error: () => {
                        showErrorDialog();
                    }
                })
            }
        })
    }
})

tippy('.update-address', {
    content: 'Cập nhật địa chỉ',
    placement: 'top'
})

tippy('.delete-address', {
    content: 'Xóa địa chỉ',
    placement: 'top'
})


$('.account-content-box .page-btn-nav-item').click(function() {
    $('.account-content-box .page-btn-nav-item').removeClass('active');
    $(this).addClass('active');
    const type = $(this).data('load-orders');

    loadOrders(type);
})