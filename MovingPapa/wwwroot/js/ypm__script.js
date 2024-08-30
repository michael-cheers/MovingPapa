document.querySelectorAll('.ypm__move-package-wrapper').forEach(package=>{
    package.addEventListener('click', function(){
        document.querySelector('.ypm__move-package-wrapper.ypm__selected-package').classList.remove('ypm__selected-package');
        package.classList.add('ypm__selected-package');
    });
});