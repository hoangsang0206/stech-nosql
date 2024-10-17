const tippyButtons = () => {
    tippy('.view-review-replies', {
        content: 'Xem phản hồi',
        placement: 'top'
    })

    tippy('.delete-review', {
        content: 'Xóa đánh giá',
        placement: 'top'
    })

    tippy('.approve-review', {
        content: 'Duyệt',
        placement: 'top'
    })

    tippy('.view-review', {
        content: 'Xem chi tiết',
        placement: 'top'
    })
}

tippyButtons();

$(document).on('click', '.approve-review', function () {
    showConfirmDialog('Phê duyệt đánh giá', 'Xác nhận duyệt đánh giá này?', () => {
        const review_id = $(this).data('review');
        const product_id = $(this).data('product');

        $.ajax({
            type: 'PATCH',
            url: `/api/admin/reviews/approve/${review_id}?pId=${product_id}`,
            success: (response) => {
                if (response.status) {
                    showDialogWithCallback('success', 'Đã phê duyệt đánh giá này', '', () => {
                        loadReviews(1);

                        $(document).remove(this);
                    })
                } else {
                    showDialog('error', 'Không thể duyệt đánh giá', response.message);
                }
            },
            error: () => {

            }
        })
    })
})

$(document).on('click', '.delete-review', function () {
    showConfirmDialog('Xóa đánh giá', 'Xác nhận xóa đánh giá này?', () => {
        const review_id = $(this).data('review');
        const product_id = $(this).data('product');

        $.ajax({
            type: 'DELETE',
            url: `/api/admin/reviews/${review_id}?pId=${product_id}`,
            success: (response) => {
                if (response.status) {
                    showDialogWithCallback('success', 'Xóa đánh giá thành công', '', () => {
                        loadReviews(1);
                        $(document).remove(this);
                    })
                } else {
                    showDialog('error', 'Không thể xóa đánh giá', response.message);
                }
            },
            error: () => {

            }
        })
    })
})

const starGroupHTML = (star) => {
    let stars = '';
    for (let i = 1; i <= star; i++) {
        stars += `<i class="fa-solid fa-star"></i>`;
    }

    for (let i = 1; i <= 5 - star; i++) {
        stars += `<i class="fa-regular fa-star"></i>`;
    }

    return `
         <div class="rating-star-group">
            ${stars}
         </div>
    `;
}

const reviewHTML = (review) => {
    const reviewerName = review.user?.fullName ?? review.reviewerName ?? "Không có tên";
    const reviewerAvatar = review.user?.avatar ?? "/admin/images/user-no-image.svg";
    const notReadCount = review.reviewReplies.filter(r => !r.isRead).length;

    return `
        <div class="review-item">
            <div class="mb-1 d-flex gap-3 align-items-center justify-content-between">
                <div class="d-flex gap-3 align-items-center">
                    <div class="reviewer-avatar">
                        <img src="${reviewerAvatar}" style="width: 2rem" alt="" />
                    </div>

                    <span class="fweight-500">
                        ${reviewerName}
                    </span>

                    ${review.isPurchased === true ?
            `<span style="padding: .05rem .6rem" class="page-badge badge-success">Đã mua sản phẩm này</span>`
            :
            `<span style ="padding: .05rem .6rem" class="page-badge badge-warning">Chưa mua sản phẩm này</span>`
        }
                </div>
                <div>
                    <span style="font-size: .85rem" class="text-secondary">${formatDateTime(review.createAt)}</span>
                </div>
            </div>

            <div class="ps-3" style="margin-left: 2rem">
                <div class="d-flex gap-3 align-items-center">
                    ${starGroupHTML(review.rating)}

                    <span class="fweight-600">"${review.content}"</span>
                </div>


                <div class="d-flex gap-3 align-items-center justify-content-between mt-2">
                    <div>
                        <span style="font-size: .87rem">
                            <span class="text-secondary">Đã đánh giá</span>
                            <a class="text-decoration-none review-product-name" href="/admin/reviews/product/${review.product.productId}">${review.product.productName}</a>
                        </span>
                    </div>

                    <div class="d-flex gap-1 align-items-stretch">
                        <div class="d-flex align-items-center">
                            <span class="me-3">(${review.reviewImages.length} hình ảnh)</span>
                        </div>

                        <div class="d-flex align-items-center">
                            <span class="me-3">(${review.reviewReplies.length} phản hồi)</span>
                        </div>

                        <button class="page-btn btn-red delete-review" data-review="${review.id}" data-product="${review.product.productId}">
                            <i class="fa-solid fa-trash-can"></i>
                        </button>
                        
                        <a href="/admin/reviews/1/${review.id}" class="page-btn btn-blue view-review">
                            <i class="fa-solid fa-ellipsis"></i>
                        </a>

                       <button class="page-btn btn-lightblue view-review-replies position-relative" data-review="${review.id}">
                            <i class="fa-regular fa-comment-dots"></i>

                            ${notReadCount > 0 ?
                                `<span class="review-replies-count position-absolute">
                                    ${notReadCount}
                                </span>` : ''
                            }
                        </button>

                        ${review.isProceeded !== true ?                       
                            `<button class="page-btn btn-green approve-review" data-review="${review.id}" data-product="${review.product.productId}">
                                <i class="fa-solid fa-check"></i>
                            </button>` : ''
                        }
                    </div>
                </div>
            </div>
        </div>
    `;
}

const loadReviews = (page) => {
    const sort_by = $('.sort-selection .page-dropdown-btn').data('selected') || null;
    const status = $('.status-selection .page-dropdown-btn').data('selected') || null;
    const filter_by = $('.filter-selection .page-dropdown-btn').data('selected') || null;
    const search = $('.search-reviews #search').val() || null;
    showWebLoader();

    $.ajax({
        type: 'GET',
        url: `/api/admin/reviews`,
        data: {
            search: search,
            page: page,
            sort_by: sort_by,
            status: status,
            filter_by: filter_by
        },
        success: (response) => {
            hideWebLoader(500);
            setTimeout(() => {
                $('.review-list').empty();

                response.data.reviews.map(review => {
                    $('.review-list').append(reviewHTML(review));
                })

                loadPagination(response.data.totalPages, response.data.currentPage);

                tippyButtons();

                updateParams({
                    search: search,
                    sort_by: sort_by,
                    status: status,
                    filter_by: filter_by
                })
            }, 500)
        },
        error: () => {

        }
    })
}



$(document).ready(() => {
    const params = new URLSearchParams(window.location.search);

    const sort_by = params.get('sort_by') || null;
    const status = params.get('status') || null;
    const filter_by = params.get('filter_by') || null;
    const search = params.get('search') || null;

    activeDropdown('.sort-selection', sort_by);
    activeDropdown('.status-selection', status);
    activeDropdown('.filter-selection', filter_by);

    $('.search-reviews #search').val(search);
})

$('.page-dropdown-item').not('.selected').click(() => {
    loadReviews(1);
})

$('.search-reviews').submit((e) => {
    e.preventDefault();
    loadReviews(1);
})

const markAllRepliesAsRead = (review_id, product_id) =>{
    $.ajax({
        type: 'PATCH',
        url: `/api/admin/reviews/mark-all-read/${review_id}?pId=${product_id}`,
        success: (response) => {
            if (response.status) {
                $(`.view-review-replies[data-review="${review_id}"] .review-replies-count`).remove();
            }
        },
    })
}

const showReviewReplies = (review_id) => {
    $.ajax({
        type: 'GET',
        url: `/api/admin/reviews/1/${review_id}`,
        success: (response) => {
            const review = response.data;
            const reviewerName = review.user?.fullName ?? review.reviewerName ?? 'Không có tên';
            const reviewerAvatar = review.user?.avatar ?? '/admin/images/user-no-image.svg';

            const currentUser = $('.page-wrapper').data('current-user');

            showForm('.reply-review');
            $('.post-reply').data('review', review_id);
            $('.post-reply').data('product', review.productId);
            $('.review').html(`
                <div class="d-flex gap-3 align-items-center">
                    <div class="reviewer-avatar">
                        <img src="${reviewerAvatar}" style="width: 2rem" alt="" />
                    </div>
                    <span class="fweight-500">
                        ${reviewerName}
                    </span>
                    ${starGroupHTML(review.rating)}
                </div>

                <div class="ms-3" style="padding-left: 2rem">
                    <span class="fweight-600">"${review.content}"</span>
                </div>
            `);

            $('.review-replies').empty();

            review.reviewReplies.map(reply => {
                $('.review-replies').append(`
                    <div class="mb-4 d-flex gap-3 ${reply.userReply.userId === currentUser ? 'flex-row-reverse' : 'justify-content-start'}">
                        <div class="reviewer-avatar" style="margin-top: 1.3rem">
                            <img src="${reply.userReply.avatar || '/admin/images/user-no-image.svg'}" alt="" />
                        </div>
                        <div style="max-width: 100%">
                            <div style="font-size: .85rem" ${reply.userReply.userId === currentUser ? 'class="text-end"' : ''}>${reply.userReply.fullName}</div>
                            <div class="review-reply-box">
                                <span>${reply.content}</span>
                            </div>
                        </div>
                    </div>
                `);
            });

            $('.review-replies').scrollTop($('.review-replies').prop("scrollHeight"));

            markAllRepliesAsRead(review_id, review.productId);
        },
        error: () => {
            console.log('Cannot get review');
        }
    })
}

$(document).on('click', '.view-review-replies', function () {
    const review_id = $(this).data('review');
    showReviewReplies(review_id);
})

$('.post-reply').submit(function (e) {
    e.preventDefault();

    const review_id = $(this).data('review');
    const product_id = $(this).data('product');
    const content = $(this).find('#txt-reply').val();

    if (!content) {
        return;
    }

    const submit_btn = $(this).find('button[type="submit"]');
    const btn_element = showButtonLoader(submit_btn, '15px', '4px');

    $.ajax({
        type: 'POST',
        url: '/api/admin/reviews/post-reply',
        contentType: 'application/json',
        data: JSON.stringify({
            ProductId: product_id,
            ReviewId: review_id,
            Content: content
        }),
        success: (response) => {
            hideButtonLoader(submit_btn, btn_element);
            if (response.status) {
                $('.review-replies').append(`
                    <div class="mb-4 d-flex gap-3 flex-row-reverse">
                        <div class="reviewer-avatar" style="margin-top: 1.3rem">
                            <img src="${response.data?.userReply.avatar || '/admin/images/user-no-image.svg'}" alt="" />
                        </div>
                        <div>
                            <div style="font-size: .85rem" class="text-end">${response.data?.userReply.fullName}</div>
                            <div class="review-reply-box">
                                ${response.data?.content}
                            </div>
                        </div>
                    </div>
                `);
                $('#txt-reply').val(null);
            } else {
                showDialog('error', 'Gửi phản hồi thất bại', response.message);
            }
        },
        error: () => {
            hideButtonLoader(submit_btn, btn_element);
        }
    })
})

$('#txt-reply').focus(() => {
    markAllRepliesAsRead($('.post-reply').data('review'));
})

$('.reload-replies').click(() => {
    showReviewReplies($('.post-reply').data('review'));
})