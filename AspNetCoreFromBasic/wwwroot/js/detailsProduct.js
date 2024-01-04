
function RedirectToBuyNow() {
    debugger;
    var count = $("#Count").val();
    var Id = $("#paraProductId").text()
    window.location = "/Customer/Home/BuyNow?Id=" + Id + "&Count=" + count
}