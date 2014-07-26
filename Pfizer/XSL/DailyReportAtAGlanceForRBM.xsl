<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="/">
		<div id="tableContainer">
			<table align="center" width="80%" cellspacing="1" valign="top"  border="1" cellpadding="0" class="inner_tb">
				<tr  class="tableContent1"  align="center" >
					<th>Date</th>
					<th>Day Name</th>
					<th>Type of Works</th>
					<th>Worked With</th>
            <th>Area / Brick</th> 
              <th>V2 Calls</th>
                <th>V3 Calls</th> 
          <th>Calling Doctors</th>
            <th>Non Calling Doctors</th>
              <th>Total Doctors</th>
                <th>Calling Chemists</th>
                  <th>Non Calling Chemists</th>
                    <th>Total Chemists</th>
                      <th>KOL</th>
                        <th>CFA/Stockist Met</th>					
				</tr>

				<xsl:choose>
					<xsl:when test="count(//row[(@Element='Activity')]) &gt;0">
						<xsl:apply-templates select="//row[@Element='Activity']" />
						<tr>
							<th></th>
             <th></th>
             <th></th>
              <th nowrap="true" align="center">
                <b>Total</b>
              </th>
             <th></th>
              <th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@V2Calls)" /></b></th>
							<th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@V3Calls)" /></b></th>
              <th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@CallDr)" /></b></th>
              <th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@NonCallDr)" /></b></th>
              <th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@TotalDr)" /></b></th>
              <th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@CallCh)" /></b></th>
							<th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@NonCallCh)" /></b></th>
              <th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@TotalCh)" /></b></th>
              <th nowrap="true" align="center"><b><xsl:value-of select="sum(//row[@Element='Activity']/@KOLFlag)" /></b></th>
							<th nowrap="true" align="center"></th>														
						</tr>
					</xsl:when>
					<xsl:otherwise>
						<tr class="tablecontent1">
							<td align="center" class="alerttext" colspan="15">No Records Found</td>
						</tr>
					</xsl:otherwise>
				</xsl:choose>
			</table>
		</div>
	</xsl:template>
	<xsl:template match="//row[@Element='Activity']">
		<xsl:variable name="ActivityDate" select="@ActivityDate"/>
		<xsl:variable name="DayName" select="@DayName"/>
		<xsl:variable name="ActivityName" select="@ActivityName"/>
		<xsl:variable name="WorkingWith" select="@PSOandJtName"/>		
		<xsl:variable name="CallDr" select="@CallDr"/>
		<xsl:variable name="V2Calls" select="@V2Calls"/>
		<xsl:variable name="V3Calls" select="@V3Calls"/>
		<xsl:variable name="NonCallDr" select="@NonCallDr"/>
		<xsl:variable name="TotalDr" select="@TotalDr"/>
		<xsl:variable name="CallCh" select="@CallCh"/>
		<xsl:variable name="NonCallCh" select="@NonCallCh"/>
		<xsl:variable name="TotalCh" select="@TotalCh"/>
		<xsl:variable name="KOLFlag" select="@KOLFlag"/>
		<xsl:variable name="CFA" select="@CFA"/>				
		<xsl:variable name="PKID" select="@PKID"/>
		<xsl:variable name="row-count" select="count(//row[@Element = 'Area' and @PKID=$PKID])" />
		<xsl:for-each select="//row[@Element='Area' and @PKID=$PKID and @ActivityDate = $ActivityDate ]">
		<xsl:variable name="row-position" select="position()"/>
		<xsl:variable name="row-span" select="count(//row[@Element = 'Area' and @PKID=$PKID and @ActivityDate = $ActivityDate])" />
			<tr>
				<xsl:if test="$row-count &lt; 2">
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$ActivityDate" /></td>
          <td class="tb-r1" nowrap="true" align="left"><xsl:value-of select="$DayName" /></td>
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$ActivityName" /></td>
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$WorkingWith" /></td>
          <td class="tb-r1" align="left" nowrap="true"><xsl:value-of select="@AreaName" /></td>
          <td class="tb-r1" align="center" nowrap="true"><xsl:value-of select="$V2Calls" /></td>
          <td class="tb-r1" align="center" nowrap="true"><xsl:value-of select="$V3Calls" /></td>
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$CallDr" /></td>
					<td  class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$NonCallDr" /></td>
          <td class="tb-r1" align="center" nowrap="true"><xsl:value-of select="$TotalDr" /></td>
          <td class="tb-r1" align="center" nowrap="true"><xsl:value-of select="$CallCh" /></td>
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$NonCallCh" /></td>
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$TotalCh" /></td>
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$KOLFlag" /></td>
          <td class="tb-r1" nowrap="true" align="center"><xsl:value-of select="$CFA" /></td>
					
				</xsl:if>
				<xsl:if test="$row-count &gt; 1">
				<xsl:choose>
					<xsl:when test="$row-position = 1" >
            <td class="tb-r1" nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$ActivityDate" />
						</td>
            <td class="tb-r1" nowrap="true" align="left">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$DayName" />
						</td>
            <td class="tb-r1" valign="center" nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$ActivityName" />
						</td>
            <td class="tb-r1" valign="center" nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$WorkingWith" />
						</td>
						
					</xsl:when>	
				</xsl:choose>
					<xsl:if test="@ActivityDate = $ActivityDate">
            <td class="tb-r1" nowrap="true" align="left"><xsl:attribute name="nowrap"/><xsl:value-of select="@AreaName" /></td>							
					</xsl:if>
					<xsl:choose>
						<xsl:when test="$row-position = 1" >
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$V2Calls" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$V3Calls" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$CallDr" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center"><xsl:attribute name="nowrap"/>
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$NonCallDr" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$TotalDr" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$CallCh" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$NonCallCh" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$TotalCh" />
						</td>
              <td class="tb-r1"  nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$KOLFlag" />
						</td>

              <td class="tb-r1" nowrap="true" align="center">
							<xsl:attribute name="rowspan" ><xsl:value-of select="$row-span" /></xsl:attribute>
							<xsl:value-of select="$CFA" />
						</td>
					</xsl:when>
					</xsl:choose>

				</xsl:if>
			</tr>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
