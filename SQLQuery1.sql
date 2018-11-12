select symbol,cast(reportDate as date),reportDate, currentAssets, currentDebt, currentAssets/currentDebt Current_Ratio  from FinancialReports
where currentDebt<>0 and symbol = 'MSFT'
order by Current_Ratio desc,symbol, cast(reportDate as date)

select symbol,reportDate from FinancialReports order by cast(reportDate as date)


select symbol,cast(reportDate as date),reportDate, currentAssets, currentDebt, currentAssets/currentDebt Current_Ratio  from FinancialReports
where currentDebt<>0 and symbol = 'MSFT'
order by Current_Ratio desc,symbol, cast(reportDate as date)

select symbol, avg(currentAssets/currentDebt) average, isnull (STDEV (currentAssets/currentDebt),0) stdeviation
from FinancialReports
Where currentDebt<>0 and (currentAssets/currentDebt) is not null
group by symbol
order by avg(currentAssets/currentDebt) desc,isnull (STDEV (currentAssets/currentDebt),0)

select currentAssets,currentDebt from FinancialReports where symbol='Z'

