$('#insurance-registeration input').change(function () {
    this.value = this.value.trim();
});

$('#insurance-registeration').validate();
$('#insurance-registeration').submit(function () {
    if (!$(this).find('input[type="checkbox"]').filter(function () { return this.checked }).length) {
        Materialize.toast('Please select at least one service', 5000);
        return false;
    }
})