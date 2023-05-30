// https://apexcharts.com/javascript-chart-demos/dashboards/modern/
const renderCharts = (modulePopularity) => {
  
    console.log("Dashboard js");
    console.log(modulePopularity);

    var modulePopularityPie = {
      series: modulePopularity.map(item => item.y),
      chart: {
        width: 380,
        type: 'pie',
      },
      labels: modulePopularity.map(item => item.x),
      responsive: [{
        breakpoint: 480,
        options: {
          chart: {
            width: 200
          },
          legend: {
            position: 'bottom',
            show: false
          }
        }
      }],
      title: {
        // margin: 10,
        offsetX: 20,
        // offsetY: 0,
        // floating: false,
        text: 'Module Popularity',
        align: 'left',
        style: {
          fontSize:  '18px',
          fontWeight:  'Semi Bold',
          fontFamily:  'Poppins',
          color:  '#263238'
        },
      },
      subtitle: {
        text: ['Determined by the discussions and ', 'completed quizzes in each Module'],
        align: 'left',
        margin: 10,
        offsetX: 20,
        offsetY: 30,
        floating: false,
        style: {
          fontSize:  '14px',
          fontWeight:  'regular',
          fontFamily:  'Poppins',
          color:  '#9699a2'
        },
      },
      legend: {
        position: 'right',
        offsetY: 100,
      },
    };

    new ApexCharts(document.querySelector("#modulePopularityPie"), modulePopularityPie).render();
    new ApexCharts(document.querySelector("#modulePopularityPie2"), modulePopularityPie).render();

//     Apex.grid = {
//       padding: { right: 0, left: 0 }
//     }
//     Apex.dataLabels = { enabled: false }  
  
};