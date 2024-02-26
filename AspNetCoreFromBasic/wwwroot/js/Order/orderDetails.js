function validateInput() {
	if (document.getElementById("trackingNumber").value == "") {
		Swal.fire({
			icon: 'error',
			title: 'Oops...',
			text: 'Please enter tracking number!',
		});
		return false;
	}
	if (document.getElementById("carrier").value == "") {
		Swal.fire({
			icon: 'error',
			title: 'Oops...',
			text: 'Please enter carrier!',
		});
		return false;
	}
	return true;
}