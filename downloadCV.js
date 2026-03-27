function downloadCv() {
    // Create a hidden <a> element and trigger a download
    const link = document.createElement('a');
    link.href = '/Leonardo_Ganas_CV.pdf';  // Path to your PDF in wwwroot folder
    link.download = 'Leonardo-Ganas-CV.pdf'; // The file name for download
    link.click();  // Trigger the download
}