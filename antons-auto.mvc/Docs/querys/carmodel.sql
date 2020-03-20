SELECT y.name, x.name, x.carmodelid
FROM carmodel as x
INNER JOIN carbrand as y
ON  x.carbrandid = y.carbrandid
ORDER BY y.name, x.name
