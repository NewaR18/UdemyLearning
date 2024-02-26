$(document).ready(function () {
	debugger;
	var clickOccurred = false;
	$(".reduceCount").on('click', function () {
		debugger;
		var totalCount = calculateTotalPrice();
		var productId = $(this).closest(".btn-group").find("p").text();
		if (productId != null || productId != "") {
			var countOfProduct = $("#txtCount" + "_" + productId).val();
			countOfProduct = countOfProduct - 1;
			var priceOfProduct = parseFloat($("#priceOfProduct" + "_" + productId).text());
			var totalPriceOfProduct = (priceOfProduct * countOfProduct).toFixed(2);
			if (countOfProduct >= 1) {
				$("#countOfProduct" + "_" + productId).text(countOfProduct);
				$("#txtCount" + "_" + productId).val(countOfProduct);
				$("#totalPriceOfProduct" + "_" + productId).text(totalPriceOfProduct)
				var grandPrice = parseFloat(calculateTotalPrice()).toFixed(2);
				$("#lblGrandTotal").text(grandPrice);
				clickOccurred = true;
				if (priceChangewithCount(countOfProduct)) {
					CallChangeCount(this);
				}
			} if (countOfProduct == 0) {
				Delete('/Customer/Cart/Delete?productId=' + productId)
			}
		} else {
			toastr.error('Error removing count from Cart')
		}
	})
	$(".increaseCount").on('click', function () {
		debugger;
		var productId = $(this).closest(".btn-group").find("p").text();
		if (productId != null || productId != "") {
			var countOfProduct = parseInt($("#txtCount" + "_" + productId).val());
			countOfProduct = 1 + countOfProduct;
			var priceOfProduct = parseFloat($("#priceOfProduct" + "_" + productId).text());
			var totalPriceOfProduct = (priceOfProduct * countOfProduct).toFixed(2);
			if (countOfProduct <= 1000) {
				$("#countOfProduct" + "_" + productId).text(countOfProduct);
				$("#txtCount" + "_" + productId).val(countOfProduct);
				$("#totalPriceOfProduct" + "_" + productId).text(totalPriceOfProduct);
				var grandPrice = parseFloat(calculateTotalPrice()).toFixed(2);
				$("#lblGrandTotal").text(grandPrice);
				clickOccurred = true;
				if (priceChangewithCount(countOfProduct)) {
					CallChangeCount(this);
				}
			}
		} else {
			toastr.error('Error reducing count from Cart')
		}
	})
	$(".reduceCount, .increaseCount").on('mouseleave', function () {
		if (clickOccurred) {
			clickOccurred = false;
			CallChangeCount(this);
		}
	})
	$(".removeProductFromCart").on('click', function () {
		debugger;
		var productId = $(this).closest(".btn-group").find("p").text();
		Delete('/Customer/Cart/Delete?productId='+productId)
	})
})
function calculateTotalPrice() {
	var total = 0;
	$(".totalPriceOfProduct").each(function () {
		total += parseFloat($(this).text());
	});
	return total
}
function Delete(url) {
	Swal.fire({
		title: "Are you sure?",
		text: "You won't be able to revert this!",
		icon: "warning",
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: "Yes, delete it!"
	}).then((result) => {
		if (result.isConfirmed) {
			$.ajax({
				url: url,
				method: "delete",
				success: function (data) {
					if (data.success) {
						location.reload();
						toastr.success(data.message)
					} else {
						toastr.error(data.message)
					}
				}
			});
		}
	});
}
function priceChangewithCount(count) {
	if (count == 49 || count == 50 || count == 99 || count == 100) {
		return true;
	}
	return false;
}
function CallChangeCount(that) {
	var productId = parseInt($(that).closest(".btn-group").find("p").text());
	var countOfProduct = $("#txtCount" + "_" + productId).val();
	$.ajax({
		url: "/Customer/Cart/ChangeCount",
		method: "post",
		datatype: "json",
		data: { productId: productId, count: countOfProduct },
		success: function (data) {
			if (data.success) {
				location.reload();
			} else {
				toastr.error(data.message)
			}
		}
	});
}