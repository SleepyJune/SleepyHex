mergeInto(LibraryManager.library, {

listFilesJS: function () {
    s3.listObjects({Bucket: bucketName, Prefix: 'SleepyHex/Resources/Levels/'}, function(err, data) {
        if (err) {
            return alert('There was an error listing your albums: ' + err.message);
        } else {
            var returnStr = JSON.stringify(data);
            SendMessage('AmazonHelpers', 'ListFileCallback', returnStr);
        }
    });
},

getObjectJS: function(filePath) {

    var params = {
              Bucket: bucketName,
              Key: Pointer_stringify(filePath)
             };

    s3.getObject(params, function(err, data) {
        if (err) {
            return alert('Error: ' + err.message);
        } else {
            var returnStr = data.Body.toString('utf-8');
            SendMessage('AmazonHelpers', 'GetFileCallback', returnStr);
        }
    });
},

deleteObjectJS: function(filePath) {

    var params = {
          Bucket: bucketName,
          Key: Pointer_stringify(filePath)
         };

    s3.deleteObject(params, function(err, data) {
        if (err) {
            return alert('Error: ' + err.message);
        } else {
            alert('Deleted');
        }
    });
},

postObjectJS: function(filePath, data){

     var params = {
      Body: Pointer_stringify(data),
      Bucket: bucketName,
      Key: Pointer_stringify(filePath)
     };

     s3.putObject(params, function(err, data) {
            if (err) {
                return alert('Error: ' + err.message);
            } else {
                alert('Uploaded');
            }
     });
},

setAlert: function (str){
    alert(Pointer_stringify(str));
}

});