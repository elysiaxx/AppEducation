
async function getUserInfo() {
    let userInfo;
    await $.ajax({
        url: "JoinClass/getInfo",
        type: "GET",
        dataType: 'json',
        data: {},
        success: function (result) {
            console.log(result);
            userInfo = result;
        }
    }).then(() => {
        console.log(userInfo);
    })
    
}