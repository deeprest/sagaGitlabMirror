
mergeInto(LibraryManager.library, {
  FileSync: function () {
    //window.alert("FileSync!");
    console.log("FileSync");
    FS.syncfs(false, function (err) {
      console.log("WRITE ERROR "+err);
    });
  }
});
