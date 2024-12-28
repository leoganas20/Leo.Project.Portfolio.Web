function downloadCv() {
    // Create a hidden <a> element and trigger a download
    const link = document.createElement('a');
    link.href = '/Leo-developer-CV.pdf';  // Path to your PDF in wwwroot folder
    link.download = 'Leo-Ganas-CV.pdf'; // The file name for download
    link.click();  // Trigger the download
}