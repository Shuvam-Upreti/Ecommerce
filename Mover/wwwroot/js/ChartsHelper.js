$(document).ready(function () {
    function generateChart(chartId, apiUrl, chartType, chartOptions) {
        $.ajax({
            url: apiUrl,
            type: 'GET',
            success: function (data) {
                var labels = data.map(item => item.label);
                var values = data.map(item => item.value);

                function generateRandomColors(count) {
                    var colors = [];
                    for (var i = 0; i < count; i++) {
                        var randomColor = 'rgba(' + [
                            Math.floor(Math.random() * 256),
                            Math.floor(Math.random() * 256),
                            Math.floor(Math.random() * 256),
                            0.2
                        ].join(',') + ')';
                        colors.push(randomColor);
                    }
                    return colors;
                }

                var backgroundColors = generateRandomColors(values.length);
                var borderColors = backgroundColors.map(color => color.replace('0.2', '1'));

                const chartData = {
                    labels: labels,
                    datasets: [{
                        label: chartOptions.label,
                        data: values,
                        backgroundColor: backgroundColors,
                        borderColor: borderColors,
                        borderWidth: 1
                    }]
                };

                const config = {
                    type: chartType,
                    data: chartData,
                    options: {
                        responsive: true,
                        plugins: {
                            legend: {
                                position: chartOptions.legendPosition || 'right',
                            },
                            title: {
                                display: true,
                                text: chartOptions.titleText || 'Chart Title'
                            }
                        }
                    },
                };

                new Chart(
                    document.getElementById(chartId),
                    config
                );
            },
            error: function (error) {
                console.error('Error fetching data', error);
            }
        });
    }

    // Export the function to be used in other scripts
    window.generateChart = generateChart;
});
